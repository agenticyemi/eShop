namespace eShop.WebhookClient.Endpoints;

// LoggerExtensions uses event id 0 for every call. LOGGEN002 wants a unique id per method;
// keeping 0 preserves the id those calls already emit.
#pragma warning disable LOGGEN002

internal static partial class WebhookEndpointsLog
{
    [LoggerMessage(EventId = 0, EventName = "ReceivedHook", Level = LogLevel.Information, Message = "Received hook with token {Token}. My token is {MyToken}. Token validation is set to {ValidateToken}")]
    public static partial void LogReceivedHook(ILogger logger, string token, string? myToken, bool validateToken);

    [LoggerMessage(EventId = 0, EventName = "ReceivedHookProcessing", Level = LogLevel.Information, Message = "Received hook is going to be processed")]
    public static partial void LogReceivedHookProcessing(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "ReceivedHookProcessed", Level = LogLevel.Information, Message = "Received hook was processed.")]
    public static partial void LogReceivedHookProcessed(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "ReceivedHookNotProcessed", Level = LogLevel.Information, Message = "Received hook is NOT processed - Bad Request returned.")]
    public static partial void LogReceivedHookNotProcessed(ILogger logger);
}

#pragma warning restore LOGGEN002
