using eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

namespace eShop.Ordering.UnitTests.Domain;

public class AddressBuilder
{
    public Address Build()
    {
        return new Address("street", "city", "state", "country", "zipcode");
    }
}

public class OrderBuilder
{
    private readonly Order order;

    public OrderBuilder(Address address)
    {
        order = new Order(
            "userId",
            "fakeName",
            address,
            cardTypeId: 5,
            cardNumber: "12",
            cardSecurityNumber: "123",
            cardHolderName: "name",
            cardExpiration: DateTime.UtcNow);
    }

    public OrderBuilder AddOne(
        int productId,
        string productName,
        decimal unitPrice,
        decimal discount,
        string pictureUrl,
        int units = 1)
    {
        order.AddOrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
        return this;
    }

    // Walks the order through the regular Submitted -> AwaitingValidation -> StockConfirmed -> Paid flow
    public OrderBuilder Paid()
    {
        order.SetAwaitingValidationStatus();
        order.SetStockConfirmedStatus();
        order.SetPaidStatus();
        return this;
    }

    public OrderBuilder Shipped()
    {
        Paid();
        order.SetShippedStatus();
        return this;
    }

    public Order Build()
    {
        return order;
    }
}
