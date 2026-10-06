namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class RequestReturnCommandHandler : IRequestHandler<RequestReturnCommand, bool>
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
    /// <returns></returns>
    public async Task<bool> Handle(RequestReturnCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

        // Shoppers can only request returns on their own orders
        var buyer = await _buyerRepository.FindAsync(command.UserId);
        if (buyer == null || orderToUpdate.BuyerId != buyer.Id)
        {
            return false;
        }

        orderToUpdate.RequestReturn(command.ProductId, command.Units, command.Reason);
        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class RequestReturnIdentifiedCommandHandler : IdentifiedCommandHandler<RequestReturnCommand, bool>
{
    public RequestReturnIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<RequestReturnCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
