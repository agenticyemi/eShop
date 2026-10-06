using System.ComponentModel.DataAnnotations;

namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

// A customer return request for a single line item of an order.
// Returns are children of the Order aggregate: they can only be created
// and transitioned through the Order aggregate root so the returnable
// units invariant is enforced in one place.
public class OrderReturn
    : Entity
{
    public int ProductId { get; private set; }

    [Required]
    public string ProductName { get; private set; }

    public int Units { get; private set; }

    [Required]
    public string Reason { get; private set; }

    public ReturnStatus Status { get; private set; }

    public DateTime RequestDate { get; private set; }

    protected OrderReturn() { }

    public OrderReturn(int productId, string productName, int units, string reason)
    {
        if (units <= 0)
        {
            throw new OrderingDomainException("Invalid number of units to return");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new OrderingDomainException("A reason is required to request a return");
        }

        ProductId = productId;
        ProductName = productName;
        Units = units;
        Reason = reason.Trim();
        Status = ReturnStatus.Requested;
        RequestDate = DateTime.UtcNow;
    }

    // Rejected returns no longer reserve units of the order item, so they can be requested again.
    public bool ReservesUnits => Status != ReturnStatus.Rejected;

    internal void SetApprovedStatus()
    {
        if (Status != ReturnStatus.Requested)
        {
            StatusChangeException(ReturnStatus.Approved);
        }

        Status = ReturnStatus.Approved;
    }

    internal void SetRejectedStatus()
    {
        if (Status != ReturnStatus.Requested)
        {
            StatusChangeException(ReturnStatus.Rejected);
        }

        Status = ReturnStatus.Rejected;
    }

    internal void SetRefundedStatus()
    {
        if (Status != ReturnStatus.Approved)
        {
            StatusChangeException(ReturnStatus.Refunded);
        }

        Status = ReturnStatus.Refunded;
    }

    private void StatusChangeException(ReturnStatus returnStatusToChange)
    {
        throw new OrderingDomainException($"Is not possible to change the return status from {Status} to {returnStatusToChange}.");
    }
}
