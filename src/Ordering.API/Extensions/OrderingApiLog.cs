namespace eShop.Ordering.API.Extensions;

// LoggerExtensions uses event id 0 for every call. LOGGEN002 wants a unique id per method;
// keeping 0 preserves the id those calls already emit. LOGGEN036 rejects object, which is how
// the original calls pass commands, results, and other values through without [LogProperties].
#pragma warning disable LOGGEN002
#pragma warning disable LOGGEN036
internal static partial class OrderingApiLog
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Trace, Message = "INSTANCE CREATED - {ClassName}")]
    public static partial void LogInstanceCreated(ILogger logger, string className);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogPublishingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, SkipEnabledCheck = false, Message = "Error publishing integration event: {IntegrationEventId}")]
    public static partial void LogErrorPublishingIntegrationEvent(ILogger logger, Exception exception, Guid integrationEventId);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})")]
    public static partial void LogEnqueuingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})")]
    public static partial void LogSendingCommand(ILogger logger, string commandName, string idProperty, object commandId, object command);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Sending command: {CommandName} - {IdProperty}: {CommandId}")]
    public static partial void LogSendingCommand(ILogger logger, string commandName, string idProperty, object commandId);

    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}")]
    public static partial void LogInvalidIntegrationEvent(ILogger logger, object integrationEvent);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "CreateOrderCommand succeeded - RequestId: {RequestId}")]
    public static partial void LogCreateOrderCommandSucceeded(ILogger logger, Guid requestId);

    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "CreateOrderCommand failed - RequestId: {RequestId}")]
    public static partial void LogCreateOrderCommandFailed(ILogger logger, Guid requestId);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})")]
    public static partial void LogCommandResult(ILogger logger, object result, string commandName, string idProperty, object commandId, object command);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Begin transaction {TransactionId} for {CommandName} ({@Command})")]
    public static partial void LogBeginTransaction(ILogger logger, Guid transactionId, string commandName, object command);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Commit transaction {TransactionId} for {CommandName}")]
    public static partial void LogCommitTransaction(ILogger logger, Guid transactionId, string commandName);

    [LoggerMessage(EventId = 0, Level = LogLevel.Error, SkipEnabledCheck = false, Message = "Error Handling transaction for {CommandName} ({@Command})")]
    public static partial void LogErrorHandlingTransaction(ILogger logger, Exception exception, string commandName, object command);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Validating command {CommandType}")]
    public static partial void LogValidatingCommand(ILogger logger, string commandType);

    [LoggerMessage(EventId = 0, Level = LogLevel.Warning, Message = "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}")]
    public static partial void LogValidationErrors(ILogger logger, string commandType, object command, string validationErrors);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Handling command {CommandName} ({@Command})")]
    public static partial void LogHandlingCommand(ILogger logger, string commandName, object command);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Command {CommandName} handled - response: {@Response}")]
    public static partial void LogCommandHandled(ILogger logger, string commandName, object response);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Creating Order - Order: {@Order}")]
    public static partial void LogCreatingOrder(ILogger logger, object order);
}
#pragma warning restore LOGGEN036
#pragma warning restore LOGGEN002
