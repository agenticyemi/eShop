namespace eShop.EventBusRabbitMQ;

internal static partial class EventBusRabbitMqTrace
{
    [LoggerMessage(Level = LogLevel.Trace, Message = "Creating RabbitMQ channel to publish event: {EventId} ({EventName})")]
    public static partial void LogCreatingRabbitMqChannelToPublishEvent(ILogger logger, Guid eventId, string eventName);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Declaring RabbitMQ exchange to publish event: {EventId}")]
    public static partial void LogDeclaringRabbitMqExchangeToPublishEvent(ILogger logger, Guid eventId);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Publishing event to RabbitMQ: {EventId}")]
    public static partial void LogPublishingEventToRabbitMq(ILogger logger, Guid eventId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Error Processing message \"{Message}\"")]
    public static partial void LogErrorProcessingMessage(ILogger logger, Exception exception, string message);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Processing RabbitMQ event: {EventName}")]
    public static partial void LogProcessingRabbitMqEvent(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Unable to resolve event type for event name {EventName}")]
    public static partial void LogUnableToResolveEventTypeForEventName(ILogger logger, string eventName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Starting RabbitMQ connection on a background thread")]
    public static partial void LogStartingRabbitMqConnectionOnBackgroundThread(ILogger logger);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Creating RabbitMQ consumer channel")]
    public static partial void LogCreatingRabbitMqConsumerChannel(ILogger logger);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Error with RabbitMQ consumer channel")]
    public static partial void LogErrorWithRabbitMqConsumerChannel(ILogger logger, Exception exception);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Starting RabbitMQ basic consume")]
    public static partial void LogStartingRabbitMqBasicConsume(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error starting RabbitMQ connection")]
    public static partial void LogErrorStartingRabbitMqConnection(ILogger logger, Exception exception);
}
