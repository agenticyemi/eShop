using eShop.Basket.API.IntegrationEvents.EventHandling.Events;

namespace eShop.Basket.API.Extensions;

// Extension-method logs use event id 0. Keep that id on every converted call.
#pragma warning disable LOGGEN002
internal static partial class BasketApiLog
{
    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Begin GetBasketById call from method {Method} for basket id {Id}")]
    public static partial void LogBeginGetBasket(ILogger logger, string method, string id);

    [LoggerMessage(EventId = 0, Level = LogLevel.Debug, Message = "Begin UpdateBasket call from method {Method} for basket id {Id}")]
    public static partial void LogBeginUpdateBasket(ILogger logger, string method, string id);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Problem occurred persisting the item.")]
    public static partial void LogBasketItemPersistenceFailed(ILogger logger);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Basket item persisted successfully.")]
    public static partial void LogBasketItemPersisted(ILogger logger);

    [LoggerMessage(EventId = 0, Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, OrderStartedIntegrationEvent integrationEvent);
}
#pragma warning restore LOGGEN002
