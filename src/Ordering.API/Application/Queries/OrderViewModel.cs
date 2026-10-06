namespace eShop.Ordering.API.Application.Queries;

public record Orderitem
{
    public string ProductName { get; init; }
    public int Units { get; init; }
    public double UnitPrice { get; init; }
    public string PictureUrl { get; init; }
}

public record Order
{
    public int OrderNumber { get; init; }
    public DateTime Date { get; init; }
    public string Status { get; init; }
    public string Description { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Zipcode { get; init; }
    public string Country { get; init; }
    public List<Orderitem> OrderItems { get; set; }
    public decimal Total { get; set; }
}

public record OrderSummary
{
    public int OrderNumber { get; init; }
    public DateTime Date { get; init; }
    public string Status { get; init; }
    public double Total { get; init; }
    // Status of the most recently requested return on the order, if any
    public string ReturnStatus { get; init; }
}

public record ReturnableItem
{
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public string PictureUrl { get; init; }
    public int Units { get; init; }
    public int ReturnableUnits { get; init; }
}

public record ReturnSummary
{
    public int ReturnNumber { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; }
    public int Units { get; init; }
    public string Reason { get; init; }
    public string Status { get; init; }
    public DateTime RequestDate { get; init; }
}

public record OrderReturns
{
    public int OrderNumber { get; init; }
    public string OrderStatus { get; init; }
    public bool CanRequestReturn { get; init; }
    public List<ReturnableItem> Items { get; init; }
    public List<ReturnSummary> Returns { get; init; }
}

public record CardType
{
    public int Id { get; init; }
    public string Name { get; init; }
}
