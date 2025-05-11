using Common.Monitoring.HealthChecks.Abstractions;

namespace Core.Services.System;

internal class BackgroundJobsHealthChecker(MaintenanceService maintenanceService) : IHealthChecker
{
    public string Name => "BackgroundJobs";

    public async Task<bool> CheckAsync(CancellationToken ct = default)
    {
        return await maintenanceService.AreBackgroundJobsEnabledAsync(ct);
    }
}