namespace eShop.Ordering.UnitTests.Domain;

using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

[TestClass]
public class OrderReturnTest
{
    [TestMethod]
    public void Request_return_on_paid_order_success()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 3);

        //Act
        var orderReturn = order.RequestReturn(productId: 1, units: 2, reason: "Wrong size");

        //Assert
        Assert.IsNotNull(orderReturn);
        Assert.AreEqual(ReturnStatus.Requested, orderReturn.Status);
        Assert.AreEqual(2, orderReturn.Units);
        Assert.AreEqual("cup", orderReturn.ProductName);
        Assert.AreEqual("Wrong size", orderReturn.Reason);
        Assert.HasCount(1, order.OrderReturns);
        Assert.AreEqual(1, order.GetReturnableUnits(1));
    }

    [TestMethod]
    public void Request_return_on_shipped_order_success()
    {
        //Arrange
        var order = new OrderBuilder(new AddressBuilder().Build())
            .AddOne(1, "cup", 10.0m, 0, string.Empty)
            .Shipped()
            .Build();

        //Act
        var orderReturn = order.RequestReturn(productId: 1, units: 1, reason: "Damaged");

        //Assert
        Assert.AreEqual(ReturnStatus.Requested, orderReturn.Status);
        Assert.AreEqual(0, order.GetReturnableUnits(1));
    }

    [TestMethod]
    public void Request_return_raises_domain_event()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);
        var eventsBeforeReturn = order.DomainEvents.Count;

        //Act
        order.RequestReturn(productId: 1, units: 1, reason: "Damaged");

        //Assert
        Assert.HasCount(eventsBeforeReturn + 1, order.DomainEvents);
        Assert.IsInstanceOfType<OrderReturnStatusChangedDomainEvent>(order.DomainEvents.Last());
    }

    [TestMethod]
    public void Request_return_on_order_that_is_not_paid_throws()
    {
        //Arrange
        var order = new OrderBuilder(new AddressBuilder().Build())
            .AddOne(1, "cup", 10.0m, 0, string.Empty)
            .Build();

        //Act - Assert
        Assert.IsFalse(order.CanRequestReturn());
        Assert.ThrowsExactly<OrderingDomainException>(() => order.RequestReturn(productId: 1, units: 1, reason: "Damaged"));
        Assert.IsEmpty(order.OrderReturns);
    }

    [TestMethod]
    public void Request_return_for_product_not_in_order_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.RequestReturn(productId: 99, units: 1, reason: "Damaged"));
    }

    [TestMethod]
    public void Request_return_for_more_units_than_ordered_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 2);

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.RequestReturn(productId: 1, units: 3, reason: "Damaged"));
    }

    [TestMethod]
    public void Request_return_for_units_already_being_returned_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 2);
        order.RequestReturn(productId: 1, units: 1, reason: "Damaged");

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.RequestReturn(productId: 1, units: 2, reason: "Changed my mind"));
        Assert.HasCount(1, order.OrderReturns);
    }

    [TestMethod]
    public void Rejected_return_releases_units_for_a_new_return()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 2);
        var orderReturn = order.RequestReturn(productId: 1, units: 2, reason: "Damaged");

        //Act
        order.RejectReturn(orderReturn.Id);

        //Assert
        Assert.AreEqual(ReturnStatus.Rejected, orderReturn.Status);
        Assert.AreEqual(2, order.GetReturnableUnits(1));
        Assert.IsNotNull(order.RequestReturn(productId: 1, units: 2, reason: "Still damaged"));
    }

    [TestMethod]
    public void Approve_and_refund_return_success()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);
        var orderReturn = order.RequestReturn(productId: 1, units: 1, reason: "Damaged");

        //Act
        order.ApproveReturn(orderReturn.Id);
        Assert.AreEqual(ReturnStatus.Approved, orderReturn.Status);

        order.RefundReturn(orderReturn.Id);

        //Assert
        Assert.AreEqual(ReturnStatus.Refunded, orderReturn.Status);
        Assert.AreEqual(0, order.GetReturnableUnits(1));
    }

    [TestMethod]
    public void Refund_return_that_is_not_approved_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);
        var orderReturn = order.RequestReturn(productId: 1, units: 1, reason: "Damaged");

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.RefundReturn(orderReturn.Id));
        Assert.AreEqual(ReturnStatus.Requested, orderReturn.Status);
    }

    [TestMethod]
    public void Approve_return_that_is_already_rejected_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);
        var orderReturn = order.RequestReturn(productId: 1, units: 1, reason: "Damaged");
        order.RejectReturn(orderReturn.Id);

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.ApproveReturn(orderReturn.Id));
    }

    [TestMethod]
    public void Approve_unknown_return_throws()
    {
        //Arrange
        var order = PaidOrderWithOneItem(productId: 1, units: 1);

        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => order.ApproveReturn(123));
    }

    [TestMethod]
    public void Invalid_number_of_units_to_return()
    {
        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => new OrderReturn(1, "cup", 0, "Damaged"));
    }

    [TestMethod]
    public void Return_without_reason_is_invalid()
    {
        //Act - Assert
        Assert.ThrowsExactly<OrderingDomainException>(() => new OrderReturn(1, "cup", 1, "   "));
    }

    private static Order PaidOrderWithOneItem(int productId, int units)
    {
        return new OrderBuilder(new AddressBuilder().Build())
            .AddOne(productId, "cup", 10.0m, 0, string.Empty, units)
            .Paid()
            .Build();
    }
}
