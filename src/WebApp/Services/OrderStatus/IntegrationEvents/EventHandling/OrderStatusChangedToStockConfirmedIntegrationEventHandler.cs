using eShop.EventBus.Abstractions;

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public class OrderStatusChangedToStockConfirmedIntegrationEventHandler(
    OrderStatusNotificationService orderStatusNotificationService,
    ILogger<OrderStatusChangedToStockConfirmedIntegrationEventHandler> logger)
    : IIntegrationEventHandler<OrderStatusChangedToStockConfirmedIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToStockConfirmedIntegrationEvent @event)
    {
        WebAppTrace.LogHandlingIntegrationEvent(logger, @event.Id, @event);
        await orderStatusNotificationService.NotifyOrderStatusChangedAsync(@event.BuyerIdentityGuid);
    }
}
