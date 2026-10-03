using System.Net;

namespace Webhooks.API.Extensions;

internal static partial class WebhooksApiTrace
{
    [LoggerMessage(EventId = 1, EventName = "GrantUrlOriginMismatch", Level = LogLevel.Warning, Message = "Url of the hook ({UrlHook} and the grant url ({Url} do not belong to same origin)")]
    public static partial void LogGrantUrlOriginMismatch(ILogger logger, string urlHook, string url);

    [LoggerMessage(EventId = 2, EventName = "SendingOptionsRequest", Level = LogLevel.Information, Message = "Sending the OPTIONS message to {Url} with token \"{Token}\"")]
    public static partial void LogSendingOptionsRequest(ILogger logger, string url, string token);

    [LoggerMessage(EventId = 3, EventName = "GrantUrlResponse", Level = LogLevel.Information, Message = "Response code is {StatusCode} for url {Url} and token in header was {TokenReceived} (expected token was {TokenExpected})")]
    public static partial void LogGrantUrlResponse(ILogger logger, HttpStatusCode statusCode, string url, string tokenReceived, string tokenExpected);

    [LoggerMessage(EventId = 4, EventName = "GrantUrlException", Level = LogLevel.Warning, Message = "Exception {TypeName} when sending OPTIONS request. Url can't be granted.")]
    public static partial void LogGrantUrlException(ILogger logger, string typeName);

    [LoggerMessage(EventId = 5, EventName = "SendingHook", Level = LogLevel.Debug, Message = "Sending hook to {DestUrl} of type {Type}")]
    public static partial void LogSendingHook(ILogger logger, string destUrl, WebhookType type);

    [LoggerMessage(EventId = 6, EventName = "OrderPaidSubscriptions", Level = LogLevel.Information, Message = "Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionsCount} subscriptions to process")]
    public static partial void LogOrderPaidSubscriptions(ILogger logger, int subscriptionsCount);

    [LoggerMessage(EventId = 7, EventName = "OrderShippedSubscriptions", Level = LogLevel.Information, Message = "Received OrderStatusChangedToShippedIntegrationEvent and got {SubscriptionCount} subscriptions to process")]
    public static partial void LogOrderShippedSubscriptions(ILogger logger, int subscriptionCount);
}
