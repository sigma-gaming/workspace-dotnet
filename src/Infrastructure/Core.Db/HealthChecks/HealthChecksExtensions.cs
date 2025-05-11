using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Db.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder RequireCoreDb(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<CoreDbHealthChecker>(
            nameof(CoreDbHealthChecker),
            ServiceLifetime.Scoped,
            true);
    }
}