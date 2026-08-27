#!/usr/bin/env bash
# Cloud Agent start phase for the eShop (.NET Aspire) reference app.
# Brings up the Docker daemon that Aspire needs to launch Postgres, Redis and
# RabbitMQ containers. Idempotent and safe to re-run: it no-ops if the daemon
# is already healthy.
set -euo pipefail

if sudo docker info >/dev/null 2>&1; then
  echo "Docker daemon already running."
else
  echo "Starting Docker daemon..."
  sudo rm -f /var/run/docker.pid
  sudo nohup dockerd >/tmp/dockerd.log 2>&1 &
  for _ in $(seq 1 60); do
    if sudo docker info >/dev/null 2>&1; then
      break
    fi
    sleep 1
  done
fi

# Let the non-root agent user reach the daemon socket without re-login.
if [ -S /var/run/docker.sock ]; then
  sudo chmod 666 /var/run/docker.sock
fi

if sudo docker info >/dev/null 2>&1; then
  echo "Docker daemon is ready."
else
  echo "Docker daemon failed to start; see /tmp/dockerd.log" >&2
  exit 1
fi
