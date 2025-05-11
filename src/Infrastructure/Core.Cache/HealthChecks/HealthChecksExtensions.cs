using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Cache.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder RequireCoreCache(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<CoreCacheHealthChecker>(
            nameof(CoreCacheHealthChecker),
            ServiceLifetime.Singleton,
            true);
    }
}