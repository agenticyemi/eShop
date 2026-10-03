using eShop.EventBus.Events;

namespace eShop.WebApp.Extensions;

// LoggerExtensions logs with event id 0. Keep that id on every method.
// LOGGEN002 rejects a shared id inside one class, which would force new ids.
#pragma warning disable LOGGEN002

internal static partial class WebAppLog
{
    [LoggerMessage(EventId = 0, EventName = "HandlingIntegrationEvent", Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 0, EventName = "ChatModel", Level = LogLevel.Debug, Message = "ChatModel: {model}")]
    public static partial void LogChatModel(ILogger logger, string? model);

    [LoggerMessage(EventId = 0, EventName = "ChatCompletionsError", Level = LogLevel.Error, Message = "Error getting chat completions.")]
    public static partial void LogChatCompletionsError(ILogger logger, Exception exception);
}

#pragma warning restore LOGGEN002
