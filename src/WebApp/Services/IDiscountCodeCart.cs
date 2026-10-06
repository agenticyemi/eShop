namespace eShop.WebApp.Services;

public interface IDiscountCodeCart
{
    AppliedDiscount? AppliedDiscount { get; }

    Task<string?> ApplyDiscountCodeAsync(string? code);

    Task RemoveDiscountCodeAsync();
}
