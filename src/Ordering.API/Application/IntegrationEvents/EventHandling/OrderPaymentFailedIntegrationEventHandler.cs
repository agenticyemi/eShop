namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class OrderPaymentFailedIntegrationEventHandler(
    IMediator mediator,
    ILogger<OrderPaymentFailedIntegrationEventHandler> logger) :
    IIntegrationEventHandler<OrderPaymentFailedIntegrationEvent>
{
    public async Task Handle(OrderPaymentFailedIntegrationEvent @event)
    {
        OrderingApiTrace.LogHandlingIntegrationEvent(logger, @event.Id, @event);

        var command = new CancelOrderCommand(@event.OrderId);

        OrderingApiTrace.LogSendingCommand(
            logger,
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await mediator.Send(command);
    }
}
