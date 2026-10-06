using System.Text.Json.Serialization;

namespace eShop.Ordering.Domain.AggregatesModel.OrderAggregate;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReturnStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    Refunded = 4
}
