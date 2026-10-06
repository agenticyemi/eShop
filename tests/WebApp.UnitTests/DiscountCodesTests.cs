using eShop.WebApp.Services;

namespace eShop.WebApp.UnitTests;

[TestClass]
public class DiscountCodesTests
{
    [TestMethod]
    public void Empty_code_is_rejected()
    {
        Assert.AreEqual("Enter a discount code.", DiscountCodes.GetError(null));
        Assert.AreEqual("Enter a discount code.", DiscountCodes.GetError("  "));
    }

    [TestMethod]
    public void Unknown_code_is_rejected()
    {
        Assert.AreEqual("That discount code is not valid.", DiscountCodes.GetError("NOPE"));
        Assert.IsNull(DiscountCodes.Canonical("NOPE"));
    }

    [TestMethod]
    public void Save10_is_accepted_regardless_of_case()
    {
        Assert.IsNull(DiscountCodes.GetError(" save10 "));
        Assert.AreEqual(DiscountCodes.Save10, DiscountCodes.Canonical("SaVe10"));

        var applied = DiscountCodes.Calculate("save10", 109.99m);

        Assert.AreEqual("SAVE10", applied.Code);
        Assert.AreEqual(11.00m, applied.Amount);
        Assert.AreEqual("SAVE10 applied. 10% off your order.", applied.StatusText);
    }

    [TestMethod]
    public void Percentage_discount_rounds_half_away_from_zero()
    {
        Assert.AreEqual(0m, DiscountCodes.AmountForRate(0m, 0.10m));
        Assert.AreEqual(0.01m, DiscountCodes.AmountForRate(0.04m, 0.10m));
        Assert.AreEqual(3.00m, DiscountCodes.AmountForRate(30m, 0.10m));
    }

    [TestMethod]
    public void Allocate_spreads_a_basket_discount_across_lines()
    {
        var allocations = DiscountCodes.Allocate(3.00m, [10m, 20m]);

        Assert.AreEqual(1.00m, allocations[0]);
        Assert.AreEqual(2.00m, allocations[1]);
        Assert.AreEqual(3.00m, allocations.Sum());
    }

    [TestMethod]
    public void Allocate_never_discounts_a_line_below_zero()
    {
        var allocations = DiscountCodes.Allocate(1.00m, [10.00m, 0.01m]);

        Assert.IsLessThanOrEqualTo(10.00m, allocations[0]);
        Assert.IsLessThanOrEqualTo(0.01m, allocations[1]);
        Assert.AreEqual(1.00m, allocations.Sum());
    }

    [TestMethod]
    public void Apply_sets_line_discounts_that_match_the_code()
    {
        List<BasketItem> items =
        [
            new() { Id = "1", ProductName = "Boots", ProductId = 1, UnitPrice = 109.99m, Quantity = 1 },
            new() { Id = "2", ProductName = "Goggles", ProductId = 3, UnitPrice = 79.99m, Quantity = 1 }
        ];
        var discount = DiscountCodes.Calculate(DiscountCodes.Save10, items.Sum(item => item.UnitPrice * item.Quantity));

        DiscountCodes.Apply(items, discount.Amount);

        Assert.AreEqual(discount.Amount, items.Sum(item => item.Discount));
        Assert.IsTrue(items.All(item => item.Discount <= item.UnitPrice * item.Quantity));
        Assert.AreEqual(170.98m, items.Sum(item => item.UnitPrice * item.Quantity - item.Discount));
    }
}