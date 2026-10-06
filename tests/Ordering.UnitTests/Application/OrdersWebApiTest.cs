namespace eShop.Ordering.UnitTests.Application;

using Microsoft.AspNetCore.Http.HttpResults;
using eShop.Ordering.API.Application.Queries;
using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;
using Order = eShop.Ordering.API.Application.Queries.Order;
using NSubstitute.ExceptionExtensions;

[TestClass]
public class OrdersWebApiTest
{
    private readonly IMediator _mediatorMock;
    private readonly IOrderQueries _orderQueriesMock;
    private readonly IIdentityService _identityServiceMock;
    private readonly ILogger<OrderServices> _loggerMock;

    public OrdersWebApiTest()
    {
        _mediatorMock = Substitute.For<IMediator>();
        _orderQueriesMock = Substitute.For<IOrderQueries>();
        _identityServiceMock = Substitute.For<IIdentityService>();
        _loggerMock = Substitute.For<ILogger<OrderServices>>();
    }

    [TestMethod]
    public async Task Cancel_order_with_requestId_success()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<CancelOrderCommand, bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.CancelOrderAsync(Guid.NewGuid(), new CancelOrderCommand(1), orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok>(result.Result);
    }

    [TestMethod]
    public async Task Cancel_order_bad_request()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<CancelOrderCommand, bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.CancelOrderAsync(Guid.Empty, new CancelOrderCommand(1), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
    }

    [TestMethod]
    public async Task Ship_order_with_requestId_success()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<ShipOrderCommand, bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.ShipOrderAsync(Guid.NewGuid(), new ShipOrderCommand(1), orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok>(result.Result);

    }

    [TestMethod]
    public async Task Ship_order_bad_request()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<CreateOrderCommand, bool>>(), default)
            .Returns(Task.FromResult(true));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.ShipOrderAsync(Guid.Empty, new ShipOrderCommand(1), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
    }

    [TestMethod]
    public async Task Request_return_with_requestId_success()
    {
        // Arrange
        _identityServiceMock.GetUserIdentity()
            .Returns(Guid.NewGuid().ToString());

        _mediatorMock.Send(Arg.Any<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult(OrderReturnResult.Success()));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.RequestReturnAsync(Guid.NewGuid(), 1, new RequestReturnRequest(1, 1, "Damaged"), orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok>(result.Result);
    }

    [TestMethod]
    public async Task Request_return_bad_request()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult(OrderReturnResult.Success()));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.RequestReturnAsync(Guid.Empty, 1, new RequestReturnRequest(1, 1, "Damaged"), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
    }

    [TestMethod]
    public async Task Request_return_domain_rule_violation_is_bad_request()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult(OrderReturnResult.Failure("Only 1 unit(s) of cup can still be returned.")));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.RequestReturnAsync(Guid.NewGuid(), 1, new RequestReturnRequest(1, 2, "Damaged"), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
        Assert.AreEqual("Only 1 unit(s) of cup can still be returned.", ((BadRequest<string>)result.Result).Value);
    }

    [TestMethod]
    public async Task Request_return_validation_error_is_bad_request()
    {
        // Arrange
        var validationFailures = new[] { new FluentValidation.Results.ValidationFailure("Reason", "A reason of at most 500 characters is required") };
#pragma warning disable NS5003
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>(), default)
            .Throws(new OrderingDomainException("Command Validation Errors", new FluentValidation.ValidationException("Validation exception", validationFailures)));
#pragma warning restore NS5003

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.RequestReturnAsync(Guid.NewGuid(), 1, new RequestReturnRequest(1, 1, ""), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
        Assert.AreEqual("A reason of at most 500 characters is required", ((BadRequest<string>)result.Result).Value);
    }

    [TestMethod]
    public async Task Request_return_failed_to_process()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult((OrderReturnResult)null));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.RequestReturnAsync(Guid.NewGuid(), 1, new RequestReturnRequest(1, 1, "Damaged"), orderServices);

        // Assert
        Assert.IsInstanceOfType<ProblemHttpResult>(result.Result);
    }

    [TestMethod]
    public async Task Update_return_status_with_requestId_success()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<UpdateReturnStatusCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult(OrderReturnResult.Success()));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.UpdateReturnStatusAsync(Guid.NewGuid(), new UpdateReturnStatusCommand(1, 1, ReturnStatus.Approved), orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok>(result.Result);
    }

    [TestMethod]
    public async Task Update_return_status_bad_request()
    {
        // Arrange
        _mediatorMock.Send(Arg.Any<IdentifiedCommand<UpdateReturnStatusCommand, OrderReturnResult>>(), default)
            .Returns(Task.FromResult(OrderReturnResult.Success()));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.UpdateReturnStatusAsync(Guid.Empty, new UpdateReturnStatusCommand(1, 1, ReturnStatus.Approved), orderServices);

        // Assert
        Assert.IsInstanceOfType<BadRequest<string>>(result.Result);
    }

    [TestMethod]
    public async Task Get_order_returns_success()
    {
        // Arrange
        var fakeOrderId = 123;
        var fakeUserId = Guid.NewGuid().ToString();
        var fakeDynamicResult = new OrderReturns();

        _identityServiceMock.GetUserIdentity()
            .Returns(fakeUserId);

        _orderQueriesMock.GetOrderReturnsAsync(fakeOrderId, fakeUserId)
            .Returns(Task.FromResult(fakeDynamicResult));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.GetOrderReturnsAsync(fakeOrderId, orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok<OrderReturns>>(result.Result);
        Assert.AreSame(fakeDynamicResult, ((Ok<OrderReturns>)result.Result).Value);
    }

    [TestMethod]
    public async Task Get_order_returns_fails()
    {
        // Arrange
        var fakeOrderId = 123;
#pragma warning disable NS5003
        _orderQueriesMock.GetOrderReturnsAsync(Arg.Any<int>(), Arg.Any<string>())
            .Throws(new KeyNotFoundException());
#pragma warning restore NS5003

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.GetOrderReturnsAsync(fakeOrderId, orderServices);

        // Assert
        Assert.IsInstanceOfType<NotFound>(result.Result);
    }

    [TestMethod]
    public async Task Get_orders_success()
    {
        // Arrange
        var fakeDynamicResult = Enumerable.Empty<OrderSummary>();

        _identityServiceMock.GetUserIdentity()
            .Returns(Guid.NewGuid().ToString());

        _orderQueriesMock.GetOrdersFromUserAsync(Guid.NewGuid().ToString())
            .Returns(Task.FromResult(fakeDynamicResult));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.GetOrdersByUserAsync(orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok<IEnumerable<OrderSummary>>>(result);
    }

    [TestMethod]
    public async Task Get_order_success()
    {
        // Arrange
        var fakeOrderId = 123;
        var fakeDynamicResult = new Order();
        _orderQueriesMock.GetOrderAsync(Arg.Any<int>())
            .Returns(Task.FromResult(fakeDynamicResult));

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.GetOrderAsync(fakeOrderId, orderServices);

        // Assert
        Assert.IsInstanceOfType<Ok<Order>>(result.Result);
        Assert.AreSame(fakeDynamicResult, ((Ok<Order>)result.Result).Value);
    }

    [TestMethod]
    public async Task Get_order_fails()
    {
        // Arrange
        var fakeOrderId = 123;
#pragma warning disable NS5003
        _orderQueriesMock.GetOrderAsync(Arg.Any<int>())
            .Throws(new KeyNotFoundException());
#pragma warning restore NS5003

        // Act
        var orderServices = new OrderServices(_mediatorMock, _orderQueriesMock, _identityServiceMock, _loggerMock);
        var result = await OrdersApi.GetOrderAsync(fakeOrderId, orderServices);

        // Assert
        Assert.IsInstanceOfType<NotFound>(result.Result);
    }

    [TestMethod]
    public async Task Get_cardTypes_success()
    {
        // Arrange
        var fakeDynamicResult = Enumerable.Empty<CardType>();
        _orderQueriesMock.GetCardTypesAsync()
            .Returns(Task.FromResult(fakeDynamicResult));

        // Act
        var result = await OrdersApi.GetCardTypesAsync(_orderQueriesMock);

        // Assert
        Assert.IsInstanceOfType<Ok<IEnumerable<CardType>>>(result);
        Assert.AreSame(fakeDynamicResult, result.Value);
    }
}
