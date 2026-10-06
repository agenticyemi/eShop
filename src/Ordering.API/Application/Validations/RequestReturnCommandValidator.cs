namespace eShop.Ordering.API.Application.Validations;

public class RequestReturnCommandValidator : AbstractValidator<RequestReturnCommand>
{
    public RequestReturnCommandValidator(ILogger<RequestReturnCommandValidator> logger)
    {
        RuleFor(command => command.OrderNumber).NotEmpty().WithMessage("No orderId found");
        RuleFor(command => command.UserId).NotEmpty().WithMessage("No userId found");
        RuleFor(command => command.ProductId).NotEmpty().WithMessage("No productId found");
        RuleFor(command => command.Units).GreaterThan(0).WithMessage("At least one unit must be returned");
        RuleFor(command => command.Reason)
            .NotEmpty().WithMessage("A reason is required to request a return")
            .MaximumLength(500).WithMessage("The reason must be at most 500 characters");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}

// Validates the wrapped command at the outer (identified) level too: failures of the inner
// command are swallowed by IdentifiedCommandHandler, while failures here reach the API as a 400.
public class RequestReturnIdentifiedCommandValidator : AbstractValidator<IdentifiedCommand<RequestReturnCommand, OrderReturnResult>>
{
    public RequestReturnIdentifiedCommandValidator(IValidator<RequestReturnCommand> commandValidator, ILogger<RequestReturnIdentifiedCommandValidator> logger)
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Command).NotNull().SetValidator(commandValidator);

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
