namespace eShop.Ordering.API.Application.IntegrationEvents.Events;

public record OrderReturnStatusChangedIntegrationEvent : IntegrationEvent
{
    public int OrderId { get; }
    public int ReturnId { get; }
    public string ProductName { get; }
    public ReturnStatus ReturnStatus { get; }
    public string BuyerName { get; }
    public string BuyerIdentityGuid { get; }

    public OrderReturnStatusChangedIntegrationEvent
        (int orderId, int returnId, string productName, ReturnStatus returnStatus, string buyerName, string buyerIdentityGuid)
    {
        OrderId = orderId;
        ReturnId = returnId;
        ProductName = productName;
        ReturnStatus = returnStatus;
        BuyerName = buyerName;
        BuyerIdentityGuid = buyerIdentityGuid;
    }
}
