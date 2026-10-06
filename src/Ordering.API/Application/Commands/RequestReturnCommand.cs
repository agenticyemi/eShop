namespace eShop.Ordering.API.Application.Commands;

public record RequestReturnCommand(int OrderNumber, string UserId, int ProductId, int Units, string Reason) : IRequest<bool>;
