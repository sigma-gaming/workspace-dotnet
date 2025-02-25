using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Nats.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder AddCoreNatsHealthCheck(
        this IHealthChecksBuilder builder,
        bool isCritical = false)
    {
        return builder.AddHealthChecker<CoreNatsHealthChecker>(
            nameof(CoreNatsHealthChecker),
            ServiceLifetime.Singleton,
            isCritical);
    }
}