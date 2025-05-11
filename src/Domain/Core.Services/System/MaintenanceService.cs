using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.System;

public class MaintenanceService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ILogger<MaintenanceService> logger)
{
    private async Task<bool> QueryMaintenanceEnabledAsync()
    {
        var config = await db.Config
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (config is null)
            throw new InvalidOperationException("Config not found");

        return config.MaintenanceEnabled;
    }

    private async Task<bool> CheckMaintenanceEnabledAsync()
    {
        var cached = await cache.MaintenanceEnabled.TryGetAsync();
        if (cached is not null)
            return cached.Value;

        var maintenance = await QueryMaintenanceEnabledAsync();
        await cache.MaintenanceEnabled.TrySetAsync(maintenance);
        return maintenance;
    }

    public async Task<bool> IsMaintenanceEnabledAsync(CancellationToken ct = default)
    {
        for (var i = 0; i < 3; i++)
            try
            {
                return await CheckMaintenanceEnabledAsync();
            }
            catch (Exception error)
            {
                logger.LogInformation("Failed to get maintenance mode");
                logger.LogError(error, "Error getting maintenance mode");
                logger.LogInformation("Retrying in 1 second...");
                await Task.Delay(1000, ct);
            }

        return true;
    }

    public async Task<bool> SetMaintenanceEnabledAsync(bool state)
    {
        await db.Config
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.MaintenanceEnabled, state)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        await cache.MaintenanceEnabled.TrySetAsync(state);
        return state;
    }

    private async Task<bool> QueryBackgroundJobsEnabledAsync(CancellationToken ct = default)
    {
        var config = await db.Config
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (config is null)
            throw new InvalidOperationException("Config not found");

        return config.BackgroundJobsEnabled;
    }

    public async Task<bool> AreBackgroundJobsEnabledAsync(CancellationToken ct = default)
    {
        var cached = await cache.BackgroundJobsEnabled.TryGetAsync();
        if (cached is not null)
            return cached.Value;

        var enabled = await QueryBackgroundJobsEnabledAsync(ct);
        await cache.BackgroundJobsEnabled.TrySetAsync(enabled);
        return enabled;
    }

    public async Task<bool> SetBackgroundJobsEnabledAsync(bool state)
    {
        await db.Config
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.BackgroundJobsEnabled, state)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        await cache.BackgroundJobsEnabled.TrySetAsync(state);
        return state;
    }
}