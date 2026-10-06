namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class RequestReturnCommandHandler : IRequestHandler<RequestReturnCommand, OrderReturnResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IBuyerRepository _buyerRepository;

    public RequestReturnCommandHandler(IOrderRepository orderRepository, IBuyerRepository buyerRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _buyerRepository = buyerRepository ?? throw new ArgumentNullException(nameof(buyerRepository));
    }

    /// <summary>
    /// Handler which processes the command when
    /// customer requests a return for one of the items of their order from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns>The result, or null when the order does not exist for this customer</returns>
    public async Task<OrderReturnResult> Handle(RequestReturnCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return null;
        }

        // Shoppers can only request returns on their own orders
        var buyer = await _buyerRepository.FindAsync(command.UserId);
        if (buyer == null || orderToUpdate.BuyerId != buyer.Id)
        {
            return null;
        }

        try
        {
            orderToUpdate.RequestReturn(command.ProductId, command.Units, command.Reason);
        }
        catch (OrderingDomainException ex)
        {
            return OrderReturnResult.Failure(ex.Message);
        }

        await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        return OrderReturnResult.Success();
    }
}


// Use for Idempotency in Command process
public class RequestReturnIdentifiedCommandHandler : IdentifiedCommandHandler<RequestReturnCommand, OrderReturnResult>
{
    public RequestReturnIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<RequestReturnCommand, OrderReturnResult>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override OrderReturnResult CreateResultForDuplicateRequest()
    {
        return OrderReturnResult.Success(); // Ignore duplicate requests for processing order.
    }
}
