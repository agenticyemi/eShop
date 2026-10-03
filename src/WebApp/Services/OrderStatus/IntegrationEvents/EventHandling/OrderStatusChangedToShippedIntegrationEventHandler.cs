using eShop.EventBus.Abstractions;

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToShippedIntegrationEventHandler(
    OrderStatusNotificationService orderStatusNotificationService,
    ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
    {
        WebAppLog.LogHandlingIntegrationEvent(logger, @event.Id, @event);
        await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
