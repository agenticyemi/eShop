namespace eShop.PaymentProcessor.Extensions;

internal static partial class PaymentProcessorLog
{
    [LoggerMessage(EventId = 0, EventName = "HandlingIntegrationEvent", Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    // LogInformation uses event id 0 for every call. Keep that id on both methods.
#pragma warning disable LOGGEN002
    [LoggerMessage(EventId = 0, EventName = "PublishingIntegrationEvent", Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogPublishingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);
#pragma warning restore LOGGEN002
}
