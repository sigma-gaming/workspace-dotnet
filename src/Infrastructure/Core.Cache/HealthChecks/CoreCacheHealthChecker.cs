using Common.Monitoring.HealthChecks.Abstractions;
using StackExchange.Redis;

namespace Core.Cache.HealthChecks;

internal class CoreCacheHealthChecker(IConnectionMultiplexer connection) : IHealthChecker
{
    public string Name => "Core Cache";

    public Task<bool> CheckAsync(CancellationToken ct = default)
    {
        return Task.FromResult(connection.IsConnected);
    }
}