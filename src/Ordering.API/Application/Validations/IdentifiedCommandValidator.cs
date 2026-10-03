namespace eShop.Ordering.API.Application.Validations;

public class IdentifiedCommandValidator : AbstractValidator<IdentifiedCommand<CreateOrderCommand, bool>>
{
    public IdentifiedCommandValidator(ILogger<IdentifiedCommandValidator> logger)
    {
        RuleFor(command => command.Id).NotEmpty();

        if (logger.IsEnabled(LogLevel.Trace))
        {
            OrderingApiTrace.LogValidatorInstanceCreated(logger, GetType().Name);
        }
    }
}
