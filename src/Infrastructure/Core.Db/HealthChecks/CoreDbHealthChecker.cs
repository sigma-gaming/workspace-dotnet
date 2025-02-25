using Common.Monitoring.HealthChecks.Abstractions;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Db.HealthChecks;

internal class CoreDbHealthChecker(CoreDbContext db) : IHealthChecker
{
    public string Name => "Core Db";

    public async Task<bool> CheckAsync(CancellationToken ct = default)
    {
        await db.Database.ExecuteSqlAsync($"SELECT 1", ct);
        return true;
    }
}