using eShop.Basket.API.Repositories;
using eShop.Basket.API.IntegrationEvents.EventHandling.Events;

namespace eShop.Basket.API.IntegrationEvents.EventHandling;

public class OrderStartedIntegrationEventHandler(
    IBasketRepository repository,
    ILogger<OrderStartedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStartedIntegrationEvent>
{
    public async Task Handle(OrderStartedIntegrationEvent @event)
    {
        BasketApiLog.LogHandlingIntegrationEvent(logger, @event.Id, @event);

        await repository.DeleteBasketAsync(@event.UserId);
    }
}
