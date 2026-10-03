using eShop.OrderProcessor.Events;

namespace eShop.OrderProcessor.Extensions;

internal static partial class OrderProcessorTrace
{
    [LoggerMessage(EventId = 1, EventName = "GracePeriodManagerStarting", Level = LogLevel.Debug, Message = "GracePeriodManagerService is starting.")]
    public static partial void LogGracePeriodManagerStarting(ILogger logger);

    [LoggerMessage(EventId = 2, EventName = "GracePeriodManagerStopping", Level = LogLevel.Debug, Message = "GracePeriodManagerService background task is stopping.")]
    public static partial void LogGracePeriodManagerStopping(ILogger logger);

    [LoggerMessage(EventId = 3, EventName = "GracePeriodManagerDoingWork", Level = LogLevel.Debug, Message = "GracePeriodManagerService background task is doing background work.")]
    public static partial void LogGracePeriodManagerDoingWork(ILogger logger);

    [LoggerMessage(EventId = 4, EventName = "CheckingConfirmedGracePeriodOrders", Level = LogLevel.Debug, Message = "Checking confirmed grace period orders")]
    public static partial void LogCheckingConfirmedGracePeriodOrders(ILogger logger);

    [LoggerMessage(EventId = 5, EventName = "PublishingIntegrationEvent", Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogPublishingIntegrationEvent(ILogger logger, Guid integrationEventId, GracePeriodConfirmedIntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 6, EventName = "DatabaseConnectionError", Level = LogLevel.Error, Message = "Fatal error establishing database connection")]
    public static partial void LogDatabaseConnectionError(ILogger logger, Exception exception);
}
