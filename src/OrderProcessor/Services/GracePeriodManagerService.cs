using eShop.EventBus.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using eShop.OrderProcessor.Events;

namespace eShop.OrderProcessor.Services
{
    public class GracePeriodManagerService(
        IOptions<BackgroundTaskOptions> options,
        IEventBus eventBus,
        ILogger<GracePeriodManagerService> logger,
        NpgsqlDataSource dataSource) : BackgroundService
    {
        private readonly BackgroundTaskOptions _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var delayTime = TimeSpan.FromSeconds(_options.CheckUpdateTime);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                OrderProcessorTrace.LogGracePeriodManagerStarting(logger);
                stoppingToken.Register(() => OrderProcessorTrace.LogGracePeriodManagerStopping(logger));
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                OrderProcessorTrace.LogGracePeriodManagerDoingWork(logger);

                await CheckConfirmedGracePeriodOrders();

                await Task.Delay(delayTime, stoppingToken);
            }

            OrderProcessorTrace.LogGracePeriodManagerStopping(logger);
        }

        private async Task CheckConfirmedGracePeriodOrders()
        {
            OrderProcessorTrace.LogCheckingConfirmedGracePeriodOrders(logger);

            var orderIds = await GetConfirmedGracePeriodOrders();

            foreach (var orderId in orderIds)
            {
                var confirmGracePeriodEvent = new GracePeriodConfirmedIntegrationEvent(orderId);

                OrderProcessorTrace.LogPublishingIntegrationEvent(logger, confirmGracePeriodEvent.Id, confirmGracePeriodEvent);

                await eventBus.PublishAsync(confirmGracePeriodEvent);
            }
        }

        private async ValueTask<List<int>> GetConfirmedGracePeriodOrders()
        {
            try
            {
                using var conn = dataSource.CreateConnection();
                using var command = conn.CreateCommand();
                command.CommandText = """
                    SELECT "Id"
                    FROM ordering.orders
                    WHERE CURRENT_TIMESTAMP - "OrderDate" >= @GracePeriodTime AND "OrderStatus" = 'Submitted'
                    """;
                command.Parameters.AddWithValue("GracePeriodTime", TimeSpan.FromMinutes(_options.GracePeriodTime));

                List<int> ids = [];

                await conn.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }

                return ids;
            }
            catch (NpgsqlException exception)
            {
                OrderProcessorTrace.LogDatabaseConnectionError(logger, exception);
            }

            return [];
        }
    }
}
