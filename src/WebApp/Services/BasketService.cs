using eShop.Basket.API.Grpc;
using GrpcBasketItem = eShop.Basket.API.Grpc.BasketItem;
using GrpcBasketClient = eShop.Basket.API.Grpc.Basket.BasketClient;

namespace eShop.WebApp.Services;

public class BasketService(GrpcBasketClient basketClient)
{
    public async Task<BasketSnapshot> GetBasketAsync()
    {
        var result = await basketClient.GetBasketAsync(new());
        return MapToBasket(result);
    }

    public async Task DeleteBasketAsync()
    {
        await basketClient.DeleteBasketAsync(new DeleteBasketRequest());
    }

    public async Task UpdateBasketAsync(IReadOnlyCollection<BasketQuantity> basket, string? discountCode)
    {
        var updatePayload = new UpdateBasketRequest
        {
            DiscountCode = discountCode ?? string.Empty,
            UpdateDiscountCode = true,
        };

        foreach (var item in basket)
        {
            updatePayload.Items.Add(new GrpcBasketItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            });
        }

        await basketClient.UpdateBasketAsync(updatePayload);
    }

    private static BasketSnapshot MapToBasket(CustomerBasketResponse response)
    {
        var items = new List<BasketQuantity>(response.Items.Count);
        foreach (var item in response.Items)
        {
            items.Add(new BasketQuantity(item.ProductId, item.Quantity));
        }

        var discountCode = string.IsNullOrWhiteSpace(response.DiscountCode) ? null : response.DiscountCode;
        return new BasketSnapshot(items, discountCode);
    }
}

public record BasketSnapshot(IReadOnlyList<BasketQuantity> Items, string? DiscountCode);

public record BasketQuantity(int ProductId, int Quantity);
