#!/usr/bin/env bash
# Cloud Agent install phase for the eShop (.NET Aspire) reference app.
# Idempotent: safe to run repeatedly and against a warm/prebuilt base image.
# Prepares toolchains and project dependencies. Per-boot services (the Docker
# daemon) are started by start.sh, not here.
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$REPO_ROOT"

DOTNET_ROOT="/usr/share/dotnet"
export PATH="$PATH:$DOTNET_ROOT"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

# 1. .NET SDK matching global.json (10.x). Installed once into a shared location.
if ! command -v dotnet >/dev/null 2>&1 || ! dotnet --list-sdks 2>/dev/null | grep -q '^10\.'; then
  echo "Installing .NET 10 SDK..."
  curl -fsSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  chmod +x /tmp/dotnet-install.sh
  sudo /tmp/dotnet-install.sh --channel 10.0 --quality ga --install-dir "$DOTNET_ROOT"
  sudo ln -sf "$DOTNET_ROOT/dotnet" /usr/local/bin/dotnet
fi

# 2. System packages: Docker engine (Aspire orchestrates Postgres/Redis/RabbitMQ
#    containers), fuse-overlayfs for nested-container storage, and the runtime
#    libraries Playwright's Chromium needs. Package names use Ubuntu 24.04 't64'
#    variants, which Playwright <1.45 install-deps does not know about.
export DEBIAN_FRONTEND=noninteractive
APT_PKGS=(
  docker.io fuse-overlayfs uidmap
  libasound2t64 libnss3 libnspr4 libatk1.0-0t64 libatk-bridge2.0-0t64
  libcups2t64 libdrm2 libdbus-1-3 libatspi2.0-0t64 libx11-6 libxcomposite1
  libxdamage1 libxext6 libxfixes3 libxrandr2 libgbm1 libxcb1 libxkbcommon0
  libpango-1.0-0 libcairo2 libgtk-3-0t64
)
missing=0
for pkg in "${APT_PKGS[@]}"; do
  dpkg -s "$pkg" >/dev/null 2>&1 || { missing=1; break; }
done
if [ "$missing" -eq 1 ]; then
  sudo apt-get update -qq
  sudo apt-get install -y -qq "${APT_PKGS[@]}"
fi

# 3. Docker daemon config. The Cloud Agent VM's root filesystem is itself an
#    overlay mount, so the default overlay2/containerd snapshotter cannot mount
#    (overlay-on-overlay fails). fuse-overlayfs works around this.
sudo mkdir -p /etc/docker
printf '%s\n' '{ "storage-driver": "fuse-overlayfs", "features": { "containerd-snapshotter": false } }' \
  | sudo tee /etc/docker/daemon.json >/dev/null
sudo usermod -aG docker "$(id -un)" || true

# 4. Restore + build the web solution (same target as CI: eShop.Web.slnf).
dotnet build eShop.Web.slnf

# 5. Playwright (Node) dependencies and the Chromium browser used by e2e tests.
npm ci
npx --yes playwright install chromium

# 6. ASP.NET Core HTTPS development certificate (used by the Aspire dashboard).
dotnet dev-certs https || true

echo "install.sh completed successfully."
