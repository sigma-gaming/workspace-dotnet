using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.System;

public static class MaintenanceExtensions
{
    public static IHealthChecksBuilder RequireDisabledMaintenance(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<MaintenanceHealthChecker>(
            nameof(MaintenanceHealthChecker),
            ServiceLifetime.Scoped,
            true);
    }

    public static IHealthChecksBuilder RequireEnabledBackgroundJobs(
        this IHealthChecksBuilder builder)
    {
        return builder.AddHealthChecker<BackgroundJobsHealthChecker>(
            nameof(BackgroundJobsHealthChecker),
            ServiceLifetime.Scoped,
            true);
    }
}