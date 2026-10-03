namespace eShop.EventBusRabbitMQ;

internal static partial class RabbitMQEventBusLog
{
    [LoggerMessage(EventId = 0, EventName = "CreatingChannelToPublish", Level = LogLevel.Trace, Message = "Creating RabbitMQ channel to publish event: {EventId} ({EventName})")]
    public static partial void LogCreatingChannelToPublish(ILogger logger, Guid eventId, string eventName);

    [LoggerMessage(EventId = 0, EventName = "DeclaringExchangeToPublish", Level = LogLevel.Trace, Message = "Declaring RabbitMQ exchange to publish event: {EventId}")]
    public static partial void LogDeclaringExchangeToPublish(ILogger logger, Guid eventId);

    [LoggerMessage(EventId = 0, EventName = "PublishingEvent", Level = LogLevel.Trace, Message = "Publishing event to RabbitMQ: {EventId}")]
    public static partial void LogPublishingEvent(ILogger logger, Guid eventId);

    [LoggerMessage(EventId = 0, EventName = "ErrorProcessingMessage", Level = LogLevel.Warning, Message = "Error Processing message \"{Message}\"")]
    public static partial void LogErrorProcessingMessage(ILogger logger, Exception exception, string message);

    [LoggerMessage(EventId = 0, EventName = "ProcessingEvent", Level = LogLevel.Trace, Message = "Processing RabbitMQ event: {EventName}")]
    public static partial void LogProcessingEvent(ILogger logger, string eventName);

    [LoggerMessage(EventId = 0, EventName = "UnableToResolveEventType", Level = LogLevel.Warning, Message = "Unable to resolve event type for event name {EventName}")]
    public static partial void LogUnableToResolveEventType(ILogger logger, string eventName);

    [LoggerMessage(EventId = 0, EventName = "StartingConnection", Level = LogLevel.Information, Message = "Starting RabbitMQ connection on a background thread")]
    public static partial void LogStartingConnection(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "CreatingConsumerChannel", Level = LogLevel.Trace, Message = "Creating RabbitMQ consumer channel")]
    public static partial void LogCreatingConsumerChannel(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "ConsumerChannelError", Level = LogLevel.Warning, Message = "Error with RabbitMQ consumer channel")]
    public static partial void LogConsumerChannelError(ILogger logger, Exception exception);

    [LoggerMessage(EventId = 0, EventName = "StartingBasicConsume", Level = LogLevel.Trace, Message = "Starting RabbitMQ basic consume")]
    public static partial void LogStartingBasicConsume(ILogger logger);

    [LoggerMessage(EventId = 0, EventName = "ErrorStartingConnection", Level = LogLevel.Error, Message = "Error starting RabbitMQ connection")]
    public static partial void LogErrorStartingConnection(ILogger logger, Exception exception);
}
