using eShop.EventBus.Events;

namespace eShop.WebApp.Extensions;

internal static partial class WebAppTrace
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Debug, Message = "ChatModel: {model}")]
    public static partial void LogChatModel(ILogger logger, string? model);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error getting chat completions.")]
    public static partial void LogChatCompletionsError(ILogger logger, Exception exception);
}
