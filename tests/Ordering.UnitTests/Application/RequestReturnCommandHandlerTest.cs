using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;
using eShop.Ordering.UnitTests.Domain;

namespace eShop.Ordering.UnitTests.Application;

[TestClass]
public class RequestReturnCommandHandlerTest
{
    private const string BuyerIdentityGuid = "9e3163b9-1ae6-4652-9dc6-7898ab7b7a00";

    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IBuyerRepository _buyerRepositoryMock;

    public RequestReturnCommandHandlerTest()
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _buyerRepositoryMock = Substitute.For<IBuyerRepository>();
    }

    [TestMethod]
    public async Task Handle_return_null_if_order_is_not_found()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<int>())
            .Returns(Task.FromResult<Order>(null));

        //Act
        var handler = new RequestReturnCommandHandler(_orderRepositoryMock, _buyerRepositoryMock);
        var result = await handler.Handle(FakeRequestReturnCommand(), CancellationToken.None);

        //Assert
        Assert.IsNull(result);
        await _orderRepositoryMock.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_return_null_if_order_does_not_belong_to_the_user()
    {
        //Arrange
        _orderRepositoryMock.GetAsync(Arg.Any<int>())
            .Returns(Task.FromResult(FakePaidOrder()));

        _buyerRepositoryMock.FindAsync(BuyerIdentityGuid)
            .Returns(Task.FromResult<Buyer>(null));

        //Act
        var handler = new RequestReturnCommandHandler(_orderRepositoryMock, _buyerRepositoryMock);
        var result = await handler.Handle(FakeRequestReturnCommand(), CancellationToken.None);

        //Assert
        Assert.IsNull(result);
        await _orderRepositoryMock.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    [TestMethod]
    public async Task Handle_requests_return_and_saves_order()
    {
        //Arrange
        var order = FakePaidOrder();

        _orderRepositoryMock.GetAsync(Arg.Any<int>())
            .Returns(Task.FromResult(order));

        _buyerRepositoryMock.FindAsync(BuyerIdentityGuid)
            .Returns(Task.FromResult(new Buyer(BuyerIdentityGuid, "fakeName")));

        _orderRepositoryMock.UnitOfWork.SaveEntitiesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(true));

        //Act
        var handler = new RequestReturnCommandHandler(_orderRepositoryMock, _buyerRepositoryMock);
        var result = await handler.Handle(FakeRequestReturnCommand(), CancellationToken.None);

        //Assert
        Assert.IsTrue(result.Succeeded);
        Assert.IsNull(result.ErrorMessage);
        Assert.HasCount(1, order.OrderReturns);
        Assert.AreEqual(ReturnStatus.Requested, order.OrderReturns.Single().Status);
    }

    [TestMethod]
    public async Task Handle_return_failure_with_reason_when_order_cannot_be_returned()
    {
        //Arrange
        var order = new OrderBuilder(new AddressBuilder().Build())
            .AddOne(1, "cup", 10.0m, 0, string.Empty, 2)
            .Build();
        order.SetPaymentMethodVerified(buyerId: 0, paymentId: 0);

        _orderRepositoryMock.GetAsync(Arg.Any<int>())
            .Returns(Task.FromResult(order));

        _buyerRepositoryMock.FindAsync(BuyerIdentityGuid)
            .Returns(Task.FromResult(new Buyer(BuyerIdentityGuid, "fakeName")));

        //Act
        var handler = new RequestReturnCommandHandler(_orderRepositoryMock, _buyerRepositoryMock);
        var result = await handler.Handle(FakeRequestReturnCommand(), CancellationToken.None);

        //Assert
        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual("Is not possible to request a return for an order in status Submitted.", result.ErrorMessage);
        Assert.IsEmpty(order.OrderReturns);
        await _orderRepositoryMock.UnitOfWork.DidNotReceive().SaveEntitiesAsync(Arg.Any<CancellationToken>());
    }

    private static RequestReturnCommand FakeRequestReturnCommand()
    {
        return new RequestReturnCommand(OrderNumber: 1, UserId: BuyerIdentityGuid, ProductId: 1, Units: 1, Reason: "Damaged");
    }

    // A transient Buyer has Id 0, so linking the order to buyer 0 makes it belong to the fake buyer
    private static Order FakePaidOrder()
    {
        var order = new OrderBuilder(new AddressBuilder().Build())
            .AddOne(1, "cup", 10.0m, 0, string.Empty, 2)
            .Paid()
            .Build();
        order.SetPaymentMethodVerified(buyerId: 0, paymentId: 0);
        return order;
    }
}
