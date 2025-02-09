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
        if (!cache.IsReady())
            return await QueryMaintenanceEnabledAsync();

        var cached = await cache.MaintenanceEnabled.GetAsync();
        if (cached is not null)
            return cached.Value;

        var maintenance = await QueryMaintenanceEnabledAsync();
        await cache.MaintenanceEnabled.SetAsync(maintenance);
        return maintenance;
    }

    public async Task<bool> IsMaintenanceEnabledAsync()
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
                await Task.Delay(1000);
            }

        return true;
    }

    public async Task<bool> SetMaintenanceEnabledAsync(bool state)
    {
        await db.Config
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.MaintenanceEnabled, state)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        if (!cache.IsReady())
            return state;

        await cache.MaintenanceEnabled.SetAsync(state);
        return state;
    }

    private async Task<bool> QueryBackgroundJobsEnabledAsync()
    {
        var config = await db.Config
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (config is null)
            throw new InvalidOperationException("Config not found");

        return config.BackgroundJobsEnabled;
    }

    public async Task<bool> AreBackgroundJobsEnabledAsync()
    {
        if (!cache.IsReady())
            return await QueryBackgroundJobsEnabledAsync();

        var cached = await cache.BackgroundJobsEnabled.GetAsync();
        if (cached is not null)
            return cached.Value;

        var enabled = await QueryBackgroundJobsEnabledAsync();
        await cache.BackgroundJobsEnabled.SetAsync(enabled);
        return enabled;
    }

    public async Task<bool> SetBackgroundJobsEnabledAsync(bool state)
    {
        await db.Config
            .ExecuteUpdateAsync(s => s
                .SetProperty(c => c.BackgroundJobsEnabled, state)
                .SetProperty(c => c.UpdatedAt, DateTime.UtcNow));

        if (!cache.IsReady())
            return state;

        await cache.BackgroundJobsEnabled.SetAsync(state);
        return state;
    }
}