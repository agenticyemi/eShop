using Microsoft.AspNetCore.Http.HttpResults;
using CardType = eShop.Ordering.API.Application.Queries.CardType;
using Order = eShop.Ordering.API.Application.Queries.Order;

public static class OrdersApi
{
    public static RouteGroupBuilder MapOrdersApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/orders").HasApiVersion(1.0);

        api.MapPut("/cancel", CancelOrderAsync);
        api.MapPut("/ship", ShipOrderAsync);
        api.MapPut("/returns/status", UpdateReturnStatusAsync);
        api.MapGet("{orderId:int}/returns", GetOrderReturnsAsync);
        api.MapPost("{orderId:int}/returns", RequestReturnAsync);
        api.MapGet("{orderId:int}", GetOrderAsync);
        api.MapGet("/", GetOrdersByUserAsync);
        api.MapGet("/cardtypes", GetCardTypesAsync);
        api.MapPost("/draft", CreateOrderDraftAsync);
        api.MapPost("/", CreateOrderAsync);

        return api;
    }

    public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> CancelOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        CancelOrderCommand command,
        [AsParameters] OrderServices services)
    {
        if (requestId == Guid.Empty)
        {
            return TypedResults.BadRequest("Empty GUID is not valid for request ID");
        }

        var requestCancelOrder = new IdentifiedCommand<CancelOrderCommand, bool>(command, requestId);

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            requestCancelOrder.GetGenericTypeName(),
            nameof(requestCancelOrder.Command.OrderNumber),
            requestCancelOrder.Command.OrderNumber,
            requestCancelOrder);

        var commandResult = await services.Mediator.Send(requestCancelOrder);

        if (!commandResult)
        {
            return TypedResults.Problem(detail: "Cancel order failed to process.", statusCode: 500);
        }

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> ShipOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        ShipOrderCommand command,
        [AsParameters] OrderServices services)
    {
        if (requestId == Guid.Empty)
        {
            return TypedResults.BadRequest("Empty GUID is not valid for request ID");
        }

        var requestShipOrder = new IdentifiedCommand<ShipOrderCommand, bool>(command, requestId);

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            requestShipOrder.GetGenericTypeName(),
            nameof(requestShipOrder.Command.OrderNumber),
            requestShipOrder.Command.OrderNumber,
            requestShipOrder);

        var commandResult = await services.Mediator.Send(requestShipOrder);

        if (!commandResult)
        {
            return TypedResults.Problem(detail: "Ship order failed to process.", statusCode: 500);
        }

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> RequestReturnAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        int orderId,
        RequestReturnRequest request,
        [AsParameters] OrderServices services)
    {
        if (requestId == Guid.Empty)
        {
            return TypedResults.BadRequest("Empty GUID is not valid for request ID");
        }

        var userId = services.IdentityService.GetUserIdentity();
        var requestReturnCommand = new RequestReturnCommand(orderId, userId, request.ProductId, request.Units, request.Reason);
        var requestReturn = new IdentifiedCommand<RequestReturnCommand, OrderReturnResult>(requestReturnCommand, requestId);

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            requestReturn.GetGenericTypeName(),
            nameof(requestReturn.Command.OrderNumber),
            requestReturn.Command.OrderNumber,
            requestReturn);

        return await SendReturnCommandAsync(services, requestReturn, "Request return failed to process.");
    }

    public static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> UpdateReturnStatusAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        UpdateReturnStatusCommand command,
        [AsParameters] OrderServices services)
    {
        if (requestId == Guid.Empty)
        {
            return TypedResults.BadRequest("Empty GUID is not valid for request ID");
        }

        var requestUpdateReturnStatus = new IdentifiedCommand<UpdateReturnStatusCommand, OrderReturnResult>(command, requestId);

        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            requestUpdateReturnStatus.GetGenericTypeName(),
            nameof(requestUpdateReturnStatus.Command.ReturnNumber),
            requestUpdateReturnStatus.Command.ReturnNumber,
            requestUpdateReturnStatus);

        return await SendReturnCommandAsync(services, requestUpdateReturnStatus, "Update return status failed to process.");
    }

    // Return commands report business rule violations through OrderReturnResult (and request validation
    // through OrderingDomainException); both are the caller's to fix, so they map to a 400 with the message.
    private static async Task<Results<Ok, BadRequest<string>, ProblemHttpResult>> SendReturnCommandAsync(
        OrderServices services,
        IRequest<OrderReturnResult> command,
        string failureDetail)
    {
        OrderReturnResult commandResult;
        try
        {
            commandResult = await services.Mediator.Send(command);
        }
        catch (OrderingDomainException ex)
        {
            return TypedResults.BadRequest(GetDomainErrorMessage(ex));
        }

        if (commandResult is null)
        {
            return TypedResults.Problem(detail: failureDetail, statusCode: 500);
        }

        if (!commandResult.Succeeded)
        {
            return TypedResults.BadRequest(commandResult.ErrorMessage);
        }

        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<OrderReturns>, NotFound>> GetOrderReturnsAsync(int orderId, [AsParameters] OrderServices services)
    {
        try
        {
            var userId = services.IdentityService.GetUserIdentity();
            var orderReturns = await services.Queries.GetOrderReturnsAsync(orderId, userId);
            return TypedResults.Ok(orderReturns);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    private static string GetDomainErrorMessage(OrderingDomainException exception)
        => exception.InnerException is ValidationException validationException
            ? string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage))
            : exception.Message;

    public static async Task<Results<Ok<Order>, NotFound>> GetOrderAsync(int orderId, [AsParameters] OrderServices services)
    {
        try
        {
            var order = await services.Queries.GetOrderAsync(orderId);
            return TypedResults.Ok(order);
        }
        catch
        {
            return TypedResults.NotFound();
        }
    }

    public static async Task<Ok<IEnumerable<OrderSummary>>> GetOrdersByUserAsync([AsParameters] OrderServices services)
    {
        var userId = services.IdentityService.GetUserIdentity();
        var orders = await services.Queries.GetOrdersFromUserAsync(userId);
        return TypedResults.Ok(orders);
    }

    public static async Task<Ok<IEnumerable<CardType>>> GetCardTypesAsync(IOrderQueries orderQueries)
    {
        var cardTypes = await orderQueries.GetCardTypesAsync();
        return TypedResults.Ok(cardTypes);
    }

    public static async Task<OrderDraftDTO> CreateOrderDraftAsync(CreateOrderDraftCommand command, [AsParameters] OrderServices services)
    {
        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
            command.GetGenericTypeName(),
            nameof(command.BuyerId),
            command.BuyerId,
            command);

        return await services.Mediator.Send(command);
    }

    public static async Task<Results<Ok, BadRequest<string>>> CreateOrderAsync(
        [FromHeader(Name = "x-requestid")] Guid requestId,
        CreateOrderRequest request,
        [AsParameters] OrderServices services)
    {
        
        //mask the credit card number
        
        services.Logger.LogInformation(
            "Sending command: {CommandName} - {IdProperty}: {CommandId}",
            request.GetGenericTypeName(),
            nameof(request.UserId),
            request.UserId); //don't log the request as it has CC number

        if (requestId == Guid.Empty)
        {
            services.Logger.LogWarning("Invalid IntegrationEvent - RequestId is missing - {@IntegrationEvent}", request);
            return TypedResults.BadRequest("RequestId is missing.");
        }

        using (services.Logger.BeginScope(new List<KeyValuePair<string, object>> { new("IdentifiedCommandId", requestId) }))
        {
            var maskedCCNumber = request.CardNumber.Substring(request.CardNumber.Length - 4).PadLeft(request.CardNumber.Length, 'X');
            var createOrderCommand = new CreateOrderCommand(request.Items, request.UserId, request.UserName, request.City, request.Street,
                request.State, request.Country, request.ZipCode,
                maskedCCNumber, request.CardHolderName, request.CardExpiration,
                request.CardSecurityNumber, request.CardTypeId);

            var requestCreateOrder = new IdentifiedCommand<CreateOrderCommand, bool>(createOrderCommand, requestId);

            services.Logger.LogInformation(
                "Sending command: {CommandName} - {IdProperty}: {CommandId} ({@Command})",
                requestCreateOrder.GetGenericTypeName(),
                nameof(requestCreateOrder.Id),
                requestCreateOrder.Id,
                requestCreateOrder);

            var result = await services.Mediator.Send(requestCreateOrder);

            if (result)
            {
                services.Logger.LogInformation("CreateOrderCommand succeeded - RequestId: {RequestId}", requestId);
            }
            else
            {
                services.Logger.LogWarning("CreateOrderCommand failed - RequestId: {RequestId}", requestId);
            }

            return TypedResults.Ok();
        }
    }
}

public record CreateOrderRequest(
    string UserId,
    string UserName,
    string City,
    string Street,
    string State,
    string Country,
    string ZipCode,
    string CardNumber,
    string CardHolderName,
    DateTime CardExpiration,
    string CardSecurityNumber,
    int CardTypeId,
    string Buyer,
    List<BasketItem> Items);

public record RequestReturnRequest(
    int ProductId,
    int Units,
    string Reason);
