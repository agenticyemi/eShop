using Webhooks.API.Extensions;

namespace Webhooks.API.Services;

class GrantUrlTesterService(IHttpClientFactory factory, ILogger<IGrantUrlTesterService> logger) : IGrantUrlTesterService
{
    public async Task<bool> TestGrantUrl(string urlHook, string url, string token)
    {
        if (!CheckSameOrigin(urlHook, url))
        {
            WebhooksApiTrace.LogGrantUrlOriginMismatch(logger, urlHook, url);
            return false;
        }

        var client = factory.CreateClient();
        var msg = new HttpRequestMessage(HttpMethod.Options, url);
        msg.Headers.Add("X-eshop-whtoken", token);

        WebhooksApiTrace.LogSendingOptionsRequest(logger, url, token ?? string.Empty);

        try
        {
            var response = await client.SendAsync(msg);
            var tokenReceived = response.Headers.TryGetValues("X-eshop-whtoken", out var tokenValues) ? tokenValues.FirstOrDefault() : null;
            var tokenExpected = string.IsNullOrWhiteSpace(token) ? null : token;

            WebhooksApiTrace.LogGrantUrlResponse(logger, response.StatusCode, url, tokenReceived, tokenExpected);

            return response.IsSuccessStatusCode && tokenReceived == tokenExpected;
        }
        catch (Exception ex)
        {
            WebhooksApiTrace.LogGrantUrlException(logger, ex.GetType().Name);

            return false;
        }
    }

    private static bool CheckSameOrigin(string urlHook, string url)
    {
        var firstUrl = new Uri(urlHook, UriKind.Absolute);
        var secondUrl = new Uri(url, UriKind.Absolute);

        return firstUrl.Scheme == secondUrl.Scheme &&
            firstUrl.Port == secondUrl.Port &&
            firstUrl.Host == secondUrl.Host;
    }
}
