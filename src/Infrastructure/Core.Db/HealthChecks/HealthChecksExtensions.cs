using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Db.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddCoreDbHealthCheck(
        this IHealthChecksBuilder builder,
        bool isCritical = false)
    {
        return builder.AddHealthChecker<CoreDbHealthChecker>(
            nameof(CoreDbHealthChecker),
            ServiceLifetime.Scoped,
            isCritical);
    }
}