using System.ComponentModel.DataAnnotations;

namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

public class Order
    : Entity, IAggregateRoot
{
    public DateTime OrderDate { get; private set; }

    // Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
    [Required]
    public Address Address { get; private set; }

    public int? BuyerId { get; private set; }

    public Buyer Buyer { get; }

    public OrderStatus OrderStatus { get; private set; }
    
    public string Description { get; private set; }

    // Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order, but we could do it if needed
#pragma warning disable CS0414 // The field 'Order._isDraft' is assigned but its value is never used
    private bool _isDraft;
#pragma warning restore CS0414

    // DDD Patterns comment
    // Using a private collection field, better for DDD Aggregate's encapsulation
    // so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
    // but only through the method OrderAggregateRoot.AddOrderItem() which includes behavior.
    private readonly List<OrderItem> _orderItems;
   
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Returns are requested per line item and, like OrderItems, are only
    // created through the aggregate root (see RequestReturn()).
    private readonly List<OrderReturn> _orderReturns;

    public IReadOnlyCollection<OrderReturn> OrderReturns => _orderReturns.AsReadOnly();

    public int? PaymentId { get; private set; }

    public static Order NewDraft()
    {
        var order = new Order
        {
            _isDraft = true
        };
        return order;
    }

    protected Order()
    {
        _orderItems = new List<OrderItem>();
        _orderReturns = new List<OrderReturn>();
        _isDraft = false;
    }

    public Order(string userId, string userName, Address address, int cardTypeId, string cardNumber, string cardSecurityNumber,
            string cardHolderName, DateTime cardExpiration, int? buyerId = null, int? paymentMethodId = null) : this()
    {
        BuyerId = buyerId;
        PaymentId = paymentMethodId;
        OrderStatus = OrderStatus.Submitted;
        OrderDate = DateTime.UtcNow;
        Address = address;

        // Add the OrderStarterDomainEvent to the domain events collection 
        // to be raised/dispatched when committing changes into the Database [ After DbContext.SaveChanges() ]
        AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
                                    cardSecurityNumber, cardHolderName, cardExpiration);
    }

    // DDD Patterns comment
    // This Order AggregateRoot's method "AddOrderItem()" should be the only way to add Items to the Order,
    // so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
    // in order to maintain consistency between the whole Aggregate. 
    public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount, string pictureUrl, int units = 1)
    {
        var existingOrderForProduct = _orderItems.SingleOrDefault(o => o.ProductId == productId);

        if (existingOrderForProduct != null)
        {
            //if previous line exist modify it with higher discount  and units..
            if (discount > existingOrderForProduct.Discount)
            {
                existingOrderForProduct.SetNewDiscount(discount);
            }

            existingOrderForProduct.AddUnits(units);
        }
        else
        {
            //add validated new order item
            var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
            _orderItems.Add(orderItem);
        }
    }

    public void SetPaymentMethodVerified(int buyerId, int paymentId)
    {
        BuyerId = buyerId;
        PaymentId = paymentId;
    }
    
    public void SetAwaitingValidationStatus()
    {
        if (OrderStatus == OrderStatus.Submitted)
        {
            AddDomainEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(Id, _orderItems));
            OrderStatus = OrderStatus.AwaitingValidation;
        }
    }

    public void SetStockConfirmedStatus()
    {
        if (OrderStatus == OrderStatus.AwaitingValidation)
        {
            AddDomainEvent(new OrderStatusChangedToStockConfirmedDomainEvent(Id));

            OrderStatus = OrderStatus.StockConfirmed;
            Description = "All the items were confirmed with available stock.";
        }
    }

    public void SetPaidStatus()
    {
        if (OrderStatus == OrderStatus.StockConfirmed)
        {
            AddDomainEvent(new OrderStatusChangedToPaidDomainEvent(Id, OrderItems));

            OrderStatus = OrderStatus.Paid;
            Description = "The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
        }
    }

    public void SetShippedStatus()
    {
        if (OrderStatus != OrderStatus.Paid)
        {
            StatusChangeException(OrderStatus.Shipped);
        }

        OrderStatus = OrderStatus.Shipped;
        Description = "The order was shipped.";
        AddDomainEvent(new OrderShippedDomainEvent(this));
    }

    public void SetCancelledStatus()
    {
        if (OrderStatus == OrderStatus.Paid ||
            OrderStatus == OrderStatus.Shipped)
        {
            StatusChangeException(OrderStatus.Cancelled);
        }

        OrderStatus = OrderStatus.Cancelled;
        Description = "The order was cancelled.";
        AddDomainEvent(new OrderCancelledDomainEvent(this));
    }

    public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
    {
        if (OrderStatus == OrderStatus.AwaitingValidation)
        {
            OrderStatus = OrderStatus.Cancelled;

            var itemsStockRejectedProductNames = OrderItems
                .Where(c => orderStockRejectedItems.Contains(c.ProductId))
                .Select(c => c.ProductName);

            var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
            Description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
        }
    }

    // Returns can only be requested once the order can no longer be cancelled,
    // i.e. once it has been paid for or shipped.
    public bool CanRequestReturn()
        => OrderStatus == OrderStatus.Paid || OrderStatus == OrderStatus.Shipped;

    // Units of an order item that have not already been claimed by a pending or completed return.
    public int GetReturnableUnits(int productId)
    {
        var orderItem = _orderItems.SingleOrDefault(o => o.ProductId == productId);
        if (orderItem is null)
        {
            return 0;
        }

        var reservedUnits = _orderReturns
            .Where(r => r.ProductId == productId && r.ReservesUnits)
            .Sum(r => r.Units);

        return orderItem.Units - reservedUnits;
    }

    public OrderReturn RequestReturn(int productId, int units, string reason)
    {
        if (!CanRequestReturn())
        {
            throw new OrderingDomainException($"Is not possible to request a return for an order in status {OrderStatus}.");
        }

        var orderItem = _orderItems.SingleOrDefault(o => o.ProductId == productId);
        if (orderItem is null)
        {
            throw new OrderingDomainException($"The order does not contain product {productId}.");
        }

        var returnableUnits = GetReturnableUnits(productId);
        if (units > returnableUnits)
        {
            throw new OrderingDomainException($"Only {returnableUnits} unit(s) of {orderItem.ProductName} can still be returned.");
        }

        var orderReturn = new OrderReturn(productId, orderItem.ProductName, units, reason);
        _orderReturns.Add(orderReturn);

        AddDomainEvent(new OrderReturnStatusChangedDomainEvent(this, orderReturn));

        return orderReturn;
    }

    public void ApproveReturn(int returnId)
    {
        var orderReturn = GetOrderReturn(returnId);
        orderReturn.SetApprovedStatus();
        AddDomainEvent(new OrderReturnStatusChangedDomainEvent(this, orderReturn));
    }

    public void RejectReturn(int returnId)
    {
        var orderReturn = GetOrderReturn(returnId);
        orderReturn.SetRejectedStatus();
        AddDomainEvent(new OrderReturnStatusChangedDomainEvent(this, orderReturn));
    }

    public void RefundReturn(int returnId)
    {
        var orderReturn = GetOrderReturn(returnId);
        orderReturn.SetRefundedStatus();
        AddDomainEvent(new OrderReturnStatusChangedDomainEvent(this, orderReturn));
    }

    private OrderReturn GetOrderReturn(int returnId)
    {
        return _orderReturns.FirstOrDefault(r => r.Id == returnId)
            ?? throw new OrderingDomainException($"The order does not contain return {returnId}.");
    }

    private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
    {
        var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
                                                                    cardNumber, cardSecurityNumber,
                                                                    cardHolderName, cardExpiration);

        this.AddDomainEvent(orderStartedDomainEvent);
    }

    private void StatusChangeException(OrderStatus orderStatusToChange)
    {
        throw new OrderingDomainException($"Is not possible to change the order status from {OrderStatus} to {orderStatusToChange}.");
    }

    public decimal GetTotal() => _orderItems.Sum(o => o.Units * o.UnitPrice);
}
