using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.RabbitMq.HealthChecks;

public static class HealthChecksExtensions
{
    public static IHealthChecksBuilder RequireCoreRabbitMq(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<CoreRabbitMqHealthChecker>(
            nameof(CoreRabbitMqHealthChecker),
            ServiceLifetime.Singleton,
            true);
    }
}