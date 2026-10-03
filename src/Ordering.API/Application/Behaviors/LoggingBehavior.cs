namespace eShop.Ordering.API.Application.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        OrderingApiTrace.LogHandlingCommand(_logger, request.GetGenericTypeName(), request);
        var response = await next();
        OrderingApiTrace.LogCommandHandled(_logger, request.GetGenericTypeName(), response);

        return response;
    }
}

