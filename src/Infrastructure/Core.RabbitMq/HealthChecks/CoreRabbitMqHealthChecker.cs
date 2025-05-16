using Common.Monitoring.HealthChecks.Abstractions;
using Common.RabbitMq;

namespace Core.RabbitMq.HealthChecks;

internal class CoreRabbitMqHealthChecker(RabbitMqConnection connection) : IHealthChecker
{
    public string Name => "Core RabbitMQ";

    public Task<bool> CheckAsync(CancellationToken ct = default)
    {
        return Task.FromResult(connection.IsOpen);
    }
}