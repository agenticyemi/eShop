using eShop.EventBus.Events;

namespace eShop.WebApp.Services.OrderStatus.IntegrationEvents;

public record OrderReturnStatusChangedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public int ReturnId { get; }
    public string ProductName { get; }
    public string ReturnStatus { get; }
    public string BuyerName { get; }
    public string BuyerIdentityGuid { get; }

    public OrderReturnStatusChangedIntegrationEvent(
        int orderId, int returnId, string productName, string returnStatus, string buyerName, string buyerIdentityGuid)
    {
        OrderId = orderId;
        ReturnId = returnId;
        ProductName = productName;
        ReturnStatus = returnStatus;
        BuyerName = buyerName;
        BuyerIdentityGuid = buyerIdentityGuid;
    }
}
