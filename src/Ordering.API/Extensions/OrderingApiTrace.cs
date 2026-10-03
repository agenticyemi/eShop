namespace eShop.Ordering.API.Extensions;

internal static partial class OrderingApiTrace
{
    [LoggerMessage(EventId = 1, EventName = "OrderStatusUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated to status {Status}")]
    public static partial void LogOrderStatusUpdated(ILogger logger, int orderId, OrderStatus status);

    [LoggerMessage(EventId = 2, EventName = "PaymentMethodUpdated", Level = LogLevel.Trace, Message = "Order with Id: {OrderId} has been successfully updated with a payment method {PaymentMethod} ({Id})")]
    public static partial void LogOrderPaymentMethodUpdated(ILogger logger, int orderId, string paymentMethod, int id);

    [LoggerMessage(EventId = 3, EventName = "BuyerAndPaymentValidatedOrUpdated", Level = LogLevel.Trace, Message = "Buyer {BuyerId} and related payment method were validated or updated for order Id: {OrderId}.")]
    public static partial void LogOrderBuyerAndPaymentValidatedOrUpdated(ILogger logger, int buyerId, int orderId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogPublishingIntegrationEvent(ILogger logger, Guid integrationEventId, [LogProperties] IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error publishing integration event: {IntegrationEventId}")]
    public static partial void LogErrorPublishingIntegrationEvent(ILogger logger, Exception exception, Guid integrationEventId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})")]
    public static partial void LogEnqueuingIntegrationEvent(ILogger logger, Guid integrationEventId, [LogProperties] IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, [LogProperties] IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Information, Message = "Sending command: {CommandName} - {IdProperty}: {CommandId}")]
    public static partial void LogSendingCommandWithoutPayload(ILogger logger, string commandName, string idProperty, string commandId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}")]
    public static partial void LogInvalidIntegrationEventMissingRequestId(ILogger logger, [LogProperties] CreateOrderRequest integrationEvent);

    [LoggerMessage(Level = LogLevel.Information, Message = "CreateOrderCommand succeeded - RequestId: {RequestId}")]
    public static partial void LogCreateOrderCommandSucceeded(ILogger logger, Guid requestId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "CreateOrderCommand failed - RequestId: {RequestId}")]
    public static partial void LogCreateOrderCommandFailed(ILogger logger, Guid requestId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Commit transaction {TransactionId} for {CommandName}")]
    public static partial void LogCommitTransaction(ILogger logger, Guid transactionId, string commandName);

    [LoggerMessage(Level = LogLevel.Information, Message = "Validating command {CommandType}")]
    public static partial void LogValidatingCommand(ILogger logger, string commandType);

    [LoggerMessage(Level = LogLevel.Trace, Message = "INSTANCE CREATED - {ClassName}")]
    public static partial void LogValidatorInstanceCreated(ILogger logger, string className);

    public static void LogSendingCommand(ILogger logger, string commandName, string idProperty, int commandId, object command) =>
        logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            commandName,
            idProperty,
            commandId,
            command);

    public static void LogSendingCommand(ILogger logger, string commandName, string idProperty, string commandId, object command) =>
        logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            commandName,
            idProperty,
            commandId,
            command);

    public static void LogSendingCommand(ILogger logger, string commandName, string idProperty, Guid commandId, object command) =>
        logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            commandName,
            idProperty,
            commandId,
            command);

    public static void LogCommandResult(ILogger logger, object result, string commandName, string idProperty, string commandId, object command) =>
        logger.LogInformation(
            "Command result: {@Result} - {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            result,
            commandName,
            idProperty,
            commandId,
            command);

    public static void LogCreatingOrder(ILogger logger, eShop.Ordering.Domain.AggregatesModel.OrderAggregate.Order order) =>
        logger.LogInformation("Creating Order - Order: {@Order}", order);

    public static void LogBeginTransaction(ILogger logger, Guid transactionId, string commandName, object command) =>
        logger.LogInformation("Begin transaction {TransactionId} for {CommandName} ({@Command})", transactionId, commandName, command);

    public static void LogErrorHandlingTransaction(ILogger logger, Exception exception, string commandName, object command) =>
        logger.LogError(exception, "Error Handling transaction for {CommandName} ({@Command})", commandName, command);

    public static void LogHandlingCommand(ILogger logger, string commandName, object command) =>
        logger.LogInformation("Handling command {CommandName} ({@Command})", commandName, command);

    public static void LogCommandHandled(ILogger logger, string commandName, object response) =>
        logger.LogInformation("Command {CommandName} handled - response: {@Response}", commandName, response);

    public static void LogValidationErrors(ILogger logger, string commandType, object command, object validationErrors) =>
        logger.LogWarning(
            "Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}",
            commandType,
            command,
            validationErrors);
}
