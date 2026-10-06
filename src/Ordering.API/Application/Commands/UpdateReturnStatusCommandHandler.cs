namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class UpdateReturnStatusCommandHandler : IRequestHandler<UpdateReturnStatusCommand, OrderReturnResult>
{
    private readonly IOrderRepository _orderRepository;

    public UpdateReturnStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
    }

    /// <summary>
    /// Handler which processes the command when
    /// administrator approves, rejects or refunds a customer return from app
    /// </summary>
    /// <param name="command"></param>
    /// <returns>The result, or null when the order does not exist</returns>
    public async Task<OrderReturnResult> Handle(UpdateReturnStatusCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return null;
        }

        try
        {
            switch (command.Status)
            {
                case ReturnStatus.Approved:
                    orderToUpdate.ApproveReturn(command.ReturnNumber);
                    break;
                case ReturnStatus.Rejected:
                    orderToUpdate.RejectReturn(command.ReturnNumber);
                    break;
                case ReturnStatus.Refunded:
                    orderToUpdate.RefundReturn(command.ReturnNumber);
                    break;
                default:
                    return OrderReturnResult.Failure($"Is not possible to set a return to status {command.Status}.");
            }
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
public class UpdateReturnStatusIdentifiedCommandHandler : IdentifiedCommandHandler<UpdateReturnStatusCommand, OrderReturnResult>
{
    public UpdateReturnStatusIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<UpdateReturnStatusCommand, OrderReturnResult>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override OrderReturnResult CreateResultForDuplicateRequest()
    {
        return OrderReturnResult.Success(); // Ignore duplicate requests for processing order.
    }
}
