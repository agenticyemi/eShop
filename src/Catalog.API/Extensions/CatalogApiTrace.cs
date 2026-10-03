namespace eShop.Catalog.API.Extensions;

internal static partial class CatalogApiTrace
{
    [LoggerMessage(EventId = 1, EventName = "IntegrationEventPublished", Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})")]
    public static partial void LogIntegrationEventPublished(ILogger logger, Guid integrationEventId_published, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 2, EventName = "IntegrationEventPublishFailed", Level = LogLevel.Error, Message = "Error Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogIntegrationEventPublishFailed(ILogger logger, Exception exception, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 3, EventName = "SavingCatalogChangesAndIntegrationEvent", Level = LogLevel.Information, Message = "CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}")]
    public static partial void LogSavingCatalogChangesAndIntegrationEvent(ILogger logger, Guid integrationEventId);

    [LoggerMessage(EventId = 4, EventName = "HandlingIntegrationEvent", Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(EventId = 5, EventName = "CatalogBrandsSeeded", Level = LogLevel.Information, Message = "Seeded catalog with {NumBrands} brands")]
    public static partial void LogCatalogBrandsSeeded(ILogger logger, int numBrands);

    [LoggerMessage(EventId = 6, EventName = "CatalogTypesSeeded", Level = LogLevel.Information, Message = "Seeded catalog with {NumTypes} types")]
    public static partial void LogCatalogTypesSeeded(ILogger logger, int numTypes);

    [LoggerMessage(EventId = 7, EventName = "GeneratingEmbeddings", Level = LogLevel.Information, Message = "Generating {NumItems} embeddings")]
    public static partial void LogGeneratingEmbeddings(ILogger logger, int numItems);

    [LoggerMessage(EventId = 8, EventName = "CatalogItemsSeeded", Level = LogLevel.Information, Message = "Seeded catalog with {NumItems} items")]
    public static partial void LogCatalogItemsSeeded(ILogger logger, int numItems);

    [LoggerMessage(EventId = 9, EventName = "SemanticSearchResults", Level = LogLevel.Debug, Message = "Results from {text}: {results}")]
    public static partial void LogSemanticSearchResults(ILogger logger, string text, string results);

    [LoggerMessage(EventId = 10, EventName = "EmbeddingsGenerated", Level = LogLevel.Trace, Message = "Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s")]
    public static partial void LogEmbeddingsGenerated(ILogger logger, int embeddingsCount, double elapsedMilliseconds);

    [LoggerMessage(EventId = 11, EventName = "EmbeddingGenerated", Level = LogLevel.Trace, Message = "Generated embedding in {ElapsedMilliseconds}s: '{Text}'")]
    public static partial void LogEmbeddingGenerated(ILogger logger, double elapsedMilliseconds, string text);
}
