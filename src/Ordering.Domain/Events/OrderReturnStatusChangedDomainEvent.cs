namespace eShop.Ordering.Domain.Events;

/// <summary>
/// Event raised whenever a return on an order is requested or moves to a new status.
/// </summary>
public class OrderReturnStatusChangedDomainEvent : INotification
{
    public Order Order { get; }
    public OrderReturn OrderReturn { get; }

    public OrderReturnStatusChangedDomainEvent(Order order, OrderReturn orderReturn)
    {
        Order = order;
        OrderReturn = orderReturn;
    }
}
