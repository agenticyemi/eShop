namespace eShop.Ordering.API.Application.Behaviors;

public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorBehavior(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = request.GetGenericTypeName();

        OrderingApiLog.LogValidatingCommand(_logger, typeName);

        var validationTasks = _validators.Select(v => v.ValidateAsync(request, cancellationToken));
        var validationResults = await Task.WhenAll(validationTasks);
        
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Any())
        {
            OrderingApiLog.LogValidationErrors(_logger, typeName, request, FormatLogEnumerable(failures));

            throw new OrderingDomainException(
                $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
        }

        return await next();
    }

    // LogValuesFormatter joins enumerables in the message. LoggerMessage calls ToString on the argument.
    private static string FormatLogEnumerable(System.Collections.IEnumerable values)
    {
        var builder = new System.Text.StringBuilder();
        var first = true;
        foreach (var value in values)
        {
            if (!first)
            {
                builder.Append(", ");
            }

            first = false;
            if (value is null)
            {
                builder.Append("(null)");
            }
            else if (value is IFormattable formattable)
            {
                builder.Append(formattable.ToString(null, System.Globalization.CultureInfo.InvariantCulture));
            }
            else
            {
                builder.Append(value.ToString());
            }
        }

        return builder.ToString();
    }
}
