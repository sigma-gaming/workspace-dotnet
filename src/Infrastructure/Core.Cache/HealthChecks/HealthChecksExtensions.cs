using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddCoreCacheHealthCheck(
        this IHealthChecksBuilder builder,
        bool isCritical = false)
    {
        return builder.AddHealthChecker<CoreCacheHealthChecker>(
            nameof(CoreCacheHealthChecker),
            ServiceLifetime.Singleton,
            isCritical);
    }
}