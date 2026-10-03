using Webhooks.API.Extensions;

namespace Webhooks.API.IntegrationEvents;

public class OrderStatusChangedToPaidIntegrationEventHandler(
    IWebhooksRetriever retriever, 
    IWebhooksSender sender, 
    ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStatusChangedToPaidIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToPaidIntegrationEvent @event)
    {
        var subscriptions = await retriever.GetSubscriptionsOfType(WebhookType.OrderPaid);

        WebhooksApiTrace.LogOrderPaidSubscriptions(logger, subscriptions.Count());

        var whook = new WebhookData(WebhookType.OrderPaid, @event);

        await sender.SendAll(subscriptions, whook);
    }
}
