namespace eShop.Ordering.API.Application.Commands;

public record UpdateReturnStatusCommand(int OrderNumber, int ReturnNumber, ReturnStatus Status) : IRequest<bool>;
