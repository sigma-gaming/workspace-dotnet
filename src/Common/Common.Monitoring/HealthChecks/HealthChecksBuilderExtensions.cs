using Common.Monitoring.HealthChecks.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Monitoring.HealthChecks;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddHealthChecker<TChecker>(
        this IHealthChecksBuilder builder,
        string name,
        ServiceLifetime checkerLifetime = ServiceLifetime.Scoped,
        bool isCritical = false) where TChecker : class, IHealthChecker
    {
        builder.Services.Add(new ServiceDescriptor(
            typeof(TChecker),
            typeof(TChecker),
            checkerLifetime));

        return builder.Add(new HealthCheckRegistration(
            name,
            sp =>
            {
                var checker = sp.GetRequiredService<TChecker>();
                return new HealthCheckerBase(checker);
            },
            null,
            isCritical ? ["ready"] : []));
    }
}