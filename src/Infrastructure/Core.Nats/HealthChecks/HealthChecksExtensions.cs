using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Nats.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder RequireCoreNats(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<CoreNatsHealthChecker>(
            nameof(CoreNatsHealthChecker),
            ServiceLifetime.Singleton,
            true);
    }
}