using Common.Monitoring.HealthChecks.Abstractions;

namespace Core.Services.System;

internal class MaintenanceHealthChecker(MaintenanceService maintenanceService) : IHealthChecker
{
    public string Name => "Maintenance";

    public async Task<bool> CheckAsync(CancellationToken ct = default)
    {
        var enabled = await maintenanceService.IsMaintenanceEnabledAsync(ct);
        return !enabled;
    }
}