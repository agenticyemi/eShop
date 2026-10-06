namespace eShop.Ordering.API.Application.Commands;

public record RequestReturnCommand(int OrderNumber, string UserId, int ProductId, int Units, string Reason) : IRequest<OrderReturnResult>;

// Result of a return command. Business rule violations are reported through ErrorMessage
// instead of an exception so the shopper gets the reason back: IdentifiedCommandHandler
// swallows exceptions thrown by the inner command and would otherwise turn them into a plain failure.
public record OrderReturnResult(bool Succeeded, string ErrorMessage)
{
    public static OrderReturnResult Success() => new(true, null);

    public static OrderReturnResult Failure(string errorMessage) => new(false, errorMessage);
}
