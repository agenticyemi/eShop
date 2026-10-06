using eShop.Ordering.API.Extensions;

namespace eShop.Ordering.UnitTests.Application;

[TestClass]
public class BasketItemExtensionsTest
{
    [TestMethod]
    public void ToOrderItemDTO_copies_discount()
    {
        var item = new BasketItem
        {
            ProductId = 5,
            ProductName = "Tent",
            UnitPrice = 40m,
            Quantity = 2,
            Discount = 8m,
            PictureUrl = "pic"
        };

        var dto = item.ToOrderItemDTO();

        Assert.AreEqual(8m, dto.Discount);
        Assert.AreEqual(40m, dto.UnitPrice);
        Assert.AreEqual(2, dto.Units);
        Assert.AreEqual("Tent", dto.ProductName);
    }
}
