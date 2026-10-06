using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using eShop.WebAppComponents.Catalog;
using eShop.WebAppComponents.Services;

namespace eShop.WebApp.Services;

public class BasketState(
    BasketService basketService,
    CatalogService catalogService,
    OrderingService orderingService,
    AuthenticationStateProvider authenticationStateProvider) : IBasketState, IDiscountCodeCart
{
    private Task<IReadOnlyCollection<BasketItem>>? _cachedBasket;
    private string? _discountCode;
    private HashSet<BasketStateChangedSubscription> _changeSubscriptions = new();

    public AppliedDiscount? AppliedDiscount { get; private set; }

    public Task DeleteBasketAsync()
        => basketService.DeleteBasketAsync();

    public async Task<IReadOnlyCollection<BasketItem>> GetBasketItemsAsync()
        => (await GetUserAsync()).Identity?.IsAuthenticated == true
        ? await FetchBasketItemsAsync()
        : [];

    public IDisposable NotifyOnChange(EventCallback callback)
    {
        var subscription = new BasketStateChangedSubscription(this, callback);
        _changeSubscriptions.Add(subscription);
        return subscription;
    }

    public async Task AddAsync(CatalogItem item)
    {
        var items = (await FetchBasketItemsAsync()).Select(i => new BasketQuantity(i.ProductId, i.Quantity)).ToList();
        bool found = false;
        for (var i = 0; i < items.Count; i++)
        {
            var existing = items[i];
            if (existing.ProductId == item.Id)
            {
                items[i] = existing with { Quantity = existing.Quantity + 1 };
                found = true;
                break;
            }
        }

        if (!found)
        {
            items.Add(new BasketQuantity(item.Id, 1));
        }

        _cachedBasket = null;
        await basketService.UpdateBasketAsync(items, _discountCode);
        await NotifyChangeSubscribersAsync();
    }

    public async Task<string?> ApplyDiscountCodeAsync(string? code)
    {
        var error = DiscountCodes.GetError(code);
        if (error is not null)
        {
            return error;
        }

        var items = (await FetchBasketItemsAsync())
            .Select(i => new BasketQuantity(i.ProductId, i.Quantity))
            .ToList();
        _cachedBasket = null;
        await basketService.UpdateBasketAsync(items, DiscountCodes.Canonical(code));
        await FetchBasketItemsAsync();
        await NotifyChangeSubscribersAsync();
        return null;
    }

    public async Task RemoveDiscountCodeAsync()
    {
        var items = (await FetchBasketItemsAsync())
            .Select(i => new BasketQuantity(i.ProductId, i.Quantity))
            .ToList();
        _cachedBasket = null;
        await basketService.UpdateBasketAsync(items, null);
        await FetchBasketItemsAsync();
        await NotifyChangeSubscribersAsync();
    }

    public async Task SetQuantityAsync(int productId, int quantity)
    {
        var existingItems = (await FetchBasketItemsAsync()).ToList();
        if (existingItems.FirstOrDefault(row => row.ProductId == productId) is { } row)
        {
            if (quantity > 0)
            {
                row.Quantity = quantity;
            }
            else
            {
                existingItems.Remove(row);
            }

            _cachedBasket = null;
            await basketService.UpdateBasketAsync(existingItems.Select(i => new BasketQuantity(i.ProductId, i.Quantity)).ToList(), _discountCode);
            await NotifyChangeSubscribersAsync();
        }
    }

    public async Task CheckoutAsync(BasketCheckoutInfo checkoutInfo)
    {
        if (checkoutInfo.RequestId == default)
        {
            checkoutInfo.RequestId = Guid.NewGuid();
        }

        var buyerId = await authenticationStateProvider.GetBuyerIdAsync() ?? throw new InvalidOperationException("User does not have a buyer ID");
        var userName = await authenticationStateProvider.GetUserNameAsync() ?? throw new InvalidOperationException("User does not have a user name");

        // Get details for the items in the basket
        var orderItems = (await FetchBasketItemsAsync()).Select(CloneBasketItem).ToList();
        if (AppliedDiscount is { Amount: > 0m } discount)
        {
            DiscountCodes.Apply(orderItems, discount.Amount);
        }

        // Call into Ordering.API to create the order using those details
        var request = new CreateOrderRequest(
            UserId: buyerId,
            UserName: userName,
            City: checkoutInfo.City!,
            Street: checkoutInfo.Street!,
            State: checkoutInfo.State!,
            Country: checkoutInfo.Country!,
            ZipCode: checkoutInfo.ZipCode!,
            CardNumber: "1111222233334444",
            CardHolderName: "TESTUSER",
            CardExpiration: DateTime.UtcNow.AddYears(1),
            CardSecurityNumber: "111",
            CardTypeId: checkoutInfo.CardTypeId,
            Buyer: buyerId,
            Items: [.. orderItems]);
        await orderingService.CreateOrder(request, checkoutInfo.RequestId);
        await DeleteBasketAsync();
    }

    private Task NotifyChangeSubscribersAsync()
        => Task.WhenAll(_changeSubscriptions.Select(s => s.NotifyAsync()));

    private async Task<ClaimsPrincipal> GetUserAsync()
        => (await authenticationStateProvider.GetAuthenticationStateAsync()).User;

    private Task<IReadOnlyCollection<BasketItem>> FetchBasketItemsAsync()
    {
        return _cachedBasket ??= FetchCoreAsync();

        async Task<IReadOnlyCollection<BasketItem>> FetchCoreAsync()
        {
            var basket = await basketService.GetBasketAsync();
            _discountCode = DiscountCodes.Canonical(basket.DiscountCode);

            if (basket.Items.Count == 0)
            {
                AppliedDiscount = _discountCode is null ? null : DiscountCodes.Calculate(_discountCode, 0m);
                return [];
            }

            // Get details for the items in the basket
            var basketItems = new List<BasketItem>();
            var productIds = basket.Items.Select(row => row.ProductId);
            var catalogItems = (await catalogService.GetCatalogItems(productIds)).ToDictionary(k => k.Id, v => v);
            foreach (var item in basket.Items)
            {
                var catalogItem = catalogItems[item.ProductId];
                var orderItem = new BasketItem
                {
                    Id = Guid.NewGuid().ToString(), // TODO: this value is meaningless, use ProductId instead.
                    ProductId = catalogItem.Id,
                    ProductName = catalogItem.Name,
                    UnitPrice = catalogItem.Price,
                    Quantity = item.Quantity,
                };
                basketItems.Add(orderItem);
            }

            var subtotal = basketItems.Sum(item => item.UnitPrice * item.Quantity);
            AppliedDiscount = _discountCode is null ? null : DiscountCodes.Calculate(_discountCode, subtotal);
            return basketItems;
        }
    }

    private static BasketItem CloneBasketItem(BasketItem item)
        => new()
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            UnitPrice = item.UnitPrice,
            OldUnitPrice = item.OldUnitPrice,
            Quantity = item.Quantity,
        };

    private class BasketStateChangedSubscription(BasketState Owner, EventCallback Callback) : IDisposable
    {
        public Task NotifyAsync() => Callback.InvokeAsync();
        public void Dispose() => Owner._changeSubscriptions.Remove(this);
    }
}

public record CreateOrderRequest(
    string UserId,
    string UserName,
    string City,
    string Street,
    string State,
    string Country,
    string ZipCode,
    string CardNumber,
    string CardHolderName,
    DateTime CardExpiration,
    string CardSecurityNumber,
    int CardTypeId,
    string Buyer,
    List<BasketItem> Items);
