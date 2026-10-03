namespace eShop.Ordering.API.Application.IntegrationEvents.EventHandling;

public class GracePeriodConfirmedIntegrationEventHandler(
    IMediator mediator,
    ILogger<GracePeriodConfirmedIntegrationEventHandler> logger) : IIntegrationEventHandler<GracePeriodConfirmedIntegrationEvent>
{
    /// <summary>
    /// Event handler which confirms that the grace period
    /// has been completed and order will not initially be cancelled.
    /// Therefore, the order process continues for validation. 
    /// </summary>
    /// <param name="event">       
    /// </param>
    /// <returns></returns>
    public async Task Handle(GracePeriodConfirmedIntegrationEvent @event)
    {
        OrderingApiTrace.LogHandlingIntegrationEvent(logger, @event.Id, @event);

        var command = new SetAwaitingValidationOrderStatusCommand(@event.OrderId);

        OrderingApiTrace.LogSendingCommand(
            logger,
            command.GetGenericTypeName(),
            nameof(command.OrderNumber),
            command.OrderNumber,
            command);

        await mediator.Send(command);
    }
}
