namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentSucceededIntegrationEventHandler(
    IMediator mediator,
    ILogger<OrderPaymentSucceededIntegrationEventHandler> logger) :
    IIntegrationEventHandler<OrderPaymentSucceededIntegrationEvent>
{
    public async Task Handle(OrderPaymentSucceededIntegrationEvent @event)
    {
        OrderingApiTrace.LogHandlingIntegrationEvent(logger, @event.Id, @event);

        var command = new SetPaidOrderStatusCommand(@event.OrderId);

        OrderingApiTrace.LogSendingCommand(
            logger,
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await mediator.Send(command);
    }
}
