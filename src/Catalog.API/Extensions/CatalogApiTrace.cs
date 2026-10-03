namespace eShop.Catalog.API.Extensions;

internal static partial class CatalogApiTrace
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Publishing integration event: {IntegrationEventId_published} - ({@IntegrationEvent})")]
    public static partial void LogPublishingIntegrationEvent(ILogger logger, Guid integrationEventId_published, IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogErrorPublishingIntegrationEvent(ILogger logger, Exception ex, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Information, Message = "CatalogIntegrationEventService - Saving changes and integrationEvent: {IntegrationEventId}")]
    public static partial void LogSavingChangesAndIntegrationEvent(ILogger logger, Guid integrationEventId);

    [LoggerMessage(Level = LogLevel.Information, Message = "Handling integration event: {IntegrationEventId} - ({@IntegrationEvent})")]
    public static partial void LogHandlingIntegrationEvent(ILogger logger, Guid integrationEventId, IntegrationEvent integrationEvent);

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded catalog with {NumBrands} brands")]
    public static partial void LogSeededCatalogWithBrands(ILogger logger, int numBrands);

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded catalog with {NumTypes} types")]
    public static partial void LogSeededCatalogWithTypes(ILogger logger, int numTypes);

    [LoggerMessage(Level = LogLevel.Information, Message = "Generating {NumItems} embeddings")]
    public static partial void LogGeneratingEmbeddings(ILogger logger, int numItems);

    [LoggerMessage(Level = LogLevel.Information, Message = "Seeded catalog with {NumItems} items")]
    public static partial void LogSeededCatalogWithItems(ILogger logger, int numItems);

    [LoggerMessage(Level = LogLevel.Debug, Message = "Results from {text}: {results}")]
    public static partial void LogSemanticSearchResults(ILogger logger, string text, string results);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Generated {EmbeddingsCount} embeddings in {ElapsedMilliseconds}s")]
    public static partial void LogGeneratedEmbeddings(ILogger logger, int embeddingsCount, double elapsedMilliseconds);

    [LoggerMessage(Level = LogLevel.Trace, Message = "Generated embedding in {ElapsedMilliseconds}s: '{Text}'")]
    public static partial void LogGeneratedEmbedding(ILogger logger, double elapsedMilliseconds, string text);
}
