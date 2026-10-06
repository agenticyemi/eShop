namespace eShop.Ordering.API.Application.Validations;

public class RequestReturnCommandValidator : AbstractValidator<RequestReturnCommand>
{
    public RequestReturnCommandValidator(ILogger<RequestReturnCommandValidator> logger)
    {
        RuleFor(command => command.OrderNumber).NotEmpty().WithMessage("No orderId found");
        RuleFor(command => command.UserId).NotEmpty().WithMessage("No userId found");
        RuleFor(command => command.ProductId).NotEmpty().WithMessage("No productId found");
        RuleFor(command => command.Units).GreaterThan(0).WithMessage("At least one unit must be returned");
        RuleFor(command => command.Reason).NotEmpty().MaximumLength(500).WithMessage("A reason of at most 500 characters is required");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
