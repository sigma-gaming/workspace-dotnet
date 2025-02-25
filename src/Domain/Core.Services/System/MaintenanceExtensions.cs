using Common.Monitoring.HealthChecks;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.System;

public static class MaintenanceExtensions
{
    public static IHealthChecksBuilder AddMaintenanceHealthCheck(
        this IHealthChecksBuilder builder,
        bool isCritical = false)
    {
        return builder.AddHealthChecker<MaintenanceHealthChecker>(
            nameof(MaintenanceHealthChecker),
            ServiceLifetime.Scoped,
            isCritical);
    }
}