namespace eShop.Ordering.API.Application.Commands;

// Regular CommandHandler
public class UpdateReturnStatusCommandHandler : IRequestHandler<UpdateReturnStatusCommand, bool>
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
    /// <returns></returns>
    public async Task<bool> Handle(UpdateReturnStatusCommand command, CancellationToken cancellationToken)
    {
        var orderToUpdate = await _orderRepository.GetAsync(command.OrderNumber);
        if (orderToUpdate == null)
        {
            return false;
        }

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
                throw new OrderingDomainException($"Is not possible to set a return to status {command.Status}.");
        }

        return await _orderRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
    }
}


// Use for Idempotency in Command process
public class UpdateReturnStatusIdentifiedCommandHandler : IdentifiedCommandHandler<UpdateReturnStatusCommand, bool>
{
    public UpdateReturnStatusIdentifiedCommandHandler(
        IMediator mediator,
        IRequestManager requestManager,
        ILogger<IdentifiedCommandHandler<UpdateReturnStatusCommand, bool>> logger)
        : base(mediator, requestManager, logger)
    {
    }

    protected override bool CreateResultForDuplicateRequest()
    {
        return true; // Ignore duplicate requests for processing order.
    }
}
