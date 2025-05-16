using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Common.RabbitMq;

public class RabbitMqConnectionHostedService(
    ILogger<RabbitMqConnectionHostedService> logger,
    RabbitMqConnection connection)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await connection.GetConnectionAsync(stoppingToken);
                break;
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(ex, "Failed to connect to RabbitMQ, retrying in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }
}