using Webhooks.API.Extensions;

namespace Webhooks.API.IntegrationEvents;

public class OrderStatusChangedToShippedIntegrationEventHandler(
    IWebhooksRetriever retriever,
    IWebhooksSender sender,
    ILogger<OrderStatusChangedToShippedIntegrationEventHandler> logger) : IIntegrationEventHandler<OrderStatusChangedToShippedIntegrationEvent>
{
    public async Task Handle(OrderStatusChangedToShippedIntegrationEvent @event)
    {
        var subscriptions = await retriever.GetSubscriptionsOfType(WebhookType.OrderShipped);

        WebhooksApiTrace.LogOrderShippedSubscriptions(logger, subscriptions.Count());

        var whook = new WebhookData(WebhookType.OrderShipped, @event);

        await sender.SendAll(subscriptions, whook);
    }
}
