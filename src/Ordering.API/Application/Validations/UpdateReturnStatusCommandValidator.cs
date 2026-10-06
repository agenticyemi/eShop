namespace eShop.Ordering.API.Application.Validations;

public class UpdateReturnStatusCommandValidator : AbstractValidator<UpdateReturnStatusCommand>
{
    public UpdateReturnStatusCommandValidator(ILogger<UpdateReturnStatusCommandValidator> logger)
    {
        RuleFor(command => command.OrderNumber).NotEmpty().WithMessage("No orderId found");
        RuleFor(command => command.ReturnNumber).NotEmpty().WithMessage("No returnId found");
        RuleFor(command => command.Status)
            .IsInEnum()
            .NotEqual(ReturnStatus.Requested)
            .WithMessage("The return status must be Approved, Rejected or Refunded");

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace("INSTANCE CREATED - {ClassName}", GetType().Name);
        }
    }
}
