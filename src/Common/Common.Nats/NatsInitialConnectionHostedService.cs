using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NATS.Client.Core;

namespace Common.Nats;

public class NatsConnectionHostedService(ILogger<NatsConnectionHostedService> logger, INatsConnection connection)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                if (connection.ConnectionState == NatsConnectionState.Open)
                {
                    logger.LogInformation("NATS connection already established");
                    break;
                }

                await connection.ConnectAsync();
                logger.LogInformation("NATS initial connection established");
                break;
            }
            catch
            {
                logger.LogError("Failed to connect to NATS, retrying in 5 seconds");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
    }
}