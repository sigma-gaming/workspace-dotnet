using Common.Monitoring.HealthChecks.Abstractions;
using NATS.Client.Core;

namespace Core.Nats.HealthChecks;

internal class CoreNatsHealthChecker(INatsConnection connection) : IHealthChecker
{
    public string Name => "Core Nats";

    public Task<bool> CheckAsync(CancellationToken ct = default)
    {
        return Task.FromResult(connection.ConnectionState == NatsConnectionState.Open);
    }
}