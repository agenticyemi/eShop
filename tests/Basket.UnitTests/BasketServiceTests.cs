using System.Security.Claims;
using eShop.Basket.API.Repositories;
using eShop.Basket.API.Grpc;
using eShop.Basket.API.Model;
using eShop.Basket.UnitTests.Helpers;
using Microsoft.Extensions.Logging.Abstractions;
using BasketItem = eShop.Basket.API.Model.BasketItem;

namespace eShop.Basket.UnitTests;

[TestClass]
public class BasketServiceTests
{
    public TestContext TestContext { get; set; }

    [TestMethod]
    public async Task GetBasketReturnsEmptyForNoUser()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        serverCallContext.SetUserState("__HttpContext", new DefaultHttpContext());

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.IsEmpty(response.Items);
    }

    [TestMethod]
    public async Task GetBasketReturnsItemsForValidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", "1")]));
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.HasCount(1, response.Items);
    }

    [TestMethod]
    public async Task GetBasketReturnsEmptyForInvalidUserId()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        List<BasketItem> items = [new BasketItem { Id = "some-id" }];
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket { BuyerId = "1", Items = items }));
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        var httpContext = new DefaultHttpContext();
        serverCallContext.SetUserState("__HttpContext", httpContext);

        var response = await service.GetBasket(new GetBasketRequest(), serverCallContext);

        Assert.IsInstanceOfType<CustomerBasketResponse>(response);
        Assert.IsEmpty(response.Items);
    }

    [TestMethod]
    public async Task GetBasketReturnsDiscountCode()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket
        {
            BuyerId = "1",
            DiscountCode = "SAVE10",
            Items = [new BasketItem { ProductId = 4, Quantity = 1 }]
        }));
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);

        var response = await service.GetBasket(new GetBasketRequest(), AuthenticatedContext("1"));

        Assert.AreEqual("SAVE10", response.DiscountCode);
        Assert.HasCount(1, response.Items);
    }

    [TestMethod]
    public async Task UpdateBasketStoresDiscountCodeWhenRequested()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        CustomerBasket saved = null!;
        mockRepository.UpdateBasketAsync(Arg.Any<CustomerBasket>()).Returns(call =>
        {
            saved = call.Arg<CustomerBasket>();
            return Task.FromResult(saved);
        });
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var request = new UpdateBasketRequest
        {
            UpdateDiscountCode = true,
            DiscountCode = " SAVE10 "
        };
        request.Items.Add(new eShop.Basket.API.Grpc.BasketItem { ProductId = 2, Quantity = 3 });

        var response = await service.UpdateBasket(request, AuthenticatedContext("1"));

        Assert.AreEqual("SAVE10", saved.DiscountCode);
        Assert.AreEqual("SAVE10", response.DiscountCode);
        Assert.AreEqual(2, saved.Items[0].ProductId);
        await mockRepository.DidNotReceive().GetBasketAsync(Arg.Any<string>());
    }

    [TestMethod]
    public async Task UpdateBasketClearsDiscountCodeWhenBlank()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        CustomerBasket saved = null!;
        mockRepository.UpdateBasketAsync(Arg.Any<CustomerBasket>()).Returns(call =>
        {
            saved = call.Arg<CustomerBasket>();
            return Task.FromResult(saved);
        });
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var request = new UpdateBasketRequest
        {
            UpdateDiscountCode = true,
            DiscountCode = "  "
        };
        request.Items.Add(new eShop.Basket.API.Grpc.BasketItem { ProductId = 2, Quantity = 1 });

        var response = await service.UpdateBasket(request, AuthenticatedContext("1"));

        Assert.IsTrue(string.IsNullOrEmpty(saved.DiscountCode));
        Assert.AreEqual(string.Empty, response.DiscountCode);
    }

    [TestMethod]
    public async Task UpdateBasketPreservesDiscountCodeWhenClientDoesNotSetOne()
    {
        var mockRepository = Substitute.For<IBasketRepository>();
        mockRepository.GetBasketAsync("1").Returns(Task.FromResult(new CustomerBasket
        {
            BuyerId = "1",
            DiscountCode = "SAVE10",
            Items = [new BasketItem { ProductId = 1, Quantity = 1 }]
        }));
        CustomerBasket saved = null!;
        mockRepository.UpdateBasketAsync(Arg.Any<CustomerBasket>()).Returns(call =>
        {
            saved = call.Arg<CustomerBasket>();
            return Task.FromResult(saved);
        });
        var service = new BasketService(mockRepository, NullLogger<BasketService>.Instance);
        var request = new UpdateBasketRequest();
        request.Items.Add(new eShop.Basket.API.Grpc.BasketItem { ProductId = 2, Quantity = 3 });

        var response = await service.UpdateBasket(request, AuthenticatedContext("1"));

        Assert.AreEqual("SAVE10", saved.DiscountCode);
        Assert.AreEqual("SAVE10", response.DiscountCode);
        Assert.AreEqual(2, saved.Items[0].ProductId);
        Assert.AreEqual(3, saved.Items[0].Quantity);
    }

    private TestServerCallContext AuthenticatedContext(string userId)
    {
        var serverCallContext = TestServerCallContext.Create(cancellationToken: TestContext.CancellationToken);
        var httpContext = new DefaultHttpContext();
        httpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("sub", userId)]));
        serverCallContext.SetUserState("__HttpContext", httpContext);
        return serverCallContext;
    }
}
