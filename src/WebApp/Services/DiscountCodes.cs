namespace eShop.WebApp.Services;

public sealed record AppliedDiscount(string Code, string Description, decimal Amount)
{
    public string StatusText => $"{Code} applied. {Description}.";
}

/// <summary>
/// Promo codes a shopper can apply to the shopping bag.
/// SAVE10 takes 10% off the bag subtotal.
/// </summary>
public static class DiscountCodes
{
    public const string Save10 = "SAVE10";

    private const decimal Save10Rate = 0.10m;

    public static string? GetError(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return "Enter a discount code.";
        }

        if (Canonical(code) is null)
        {
            return "That discount code is not valid.";
        }

        return null;
    }

    public static string? Canonical(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return string.Equals(code.Trim(), Save10, StringComparison.OrdinalIgnoreCase)
            ? Save10
            : null;
    }

    public static AppliedDiscount Calculate(string code, decimal subtotal)
    {
        var canonical = Canonical(code) ?? throw new ArgumentException("Unknown discount code.", nameof(code));
        return new AppliedDiscount(canonical, "10% off your order", AmountForRate(subtotal, Save10Rate));
    }

    public static decimal AmountForRate(decimal subtotal, decimal rate)
    {
        if (subtotal <= 0m || rate <= 0m)
        {
            return 0m;
        }

        var amount = decimal.Round(subtotal * rate, 2, MidpointRounding.AwayFromZero);
        if (amount == 0m)
        {
            amount = 0.01m;
        }

        return amount > subtotal ? subtotal : amount;
    }

    public static decimal[] Allocate(decimal discount, IReadOnlyList<decimal> lineTotals)
    {
        var allocations = new decimal[lineTotals.Count];
        if (discount <= 0m || lineTotals.Count == 0)
        {
            return allocations;
        }

        var remainingDiscount = discount > lineTotals.Sum() ? lineTotals.Sum() : discount;
        var remainingSubtotal = lineTotals.Sum();

        for (var i = 0; i < lineTotals.Count; i++)
        {
            var line = lineTotals[i];
            decimal share;
            if (i == lineTotals.Count - 1 || remainingSubtotal == 0m)
            {
                share = line < remainingDiscount ? line : remainingDiscount;
            }
            else
            {
                share = decimal.Round(remainingDiscount * line / remainingSubtotal, 2, MidpointRounding.AwayFromZero);
                if (share > line)
                {
                    share = line;
                }

                if (share > remainingDiscount)
                {
                    share = remainingDiscount;
                }
            }

            if (share < 0m)
            {
                share = 0m;
            }

            allocations[i] = share;
            remainingDiscount -= share;
            remainingSubtotal -= line;
        }

        return allocations;
    }

    public static void Apply(IList<BasketItem> items, decimal discount)
    {
        var allocations = Allocate(discount, items.Select(item => item.UnitPrice * item.Quantity).ToList());
        for (var i = 0; i < items.Count; i++)
        {
            items[i].Discount = allocations[i];
        }
    }
}
