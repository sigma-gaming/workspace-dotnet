using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Z.EntityFramework.Plus;

namespace Core.Services.Finance;

public class BudgetService(
    ILogger<BudgetService> logger,
    CoreCacheRegistry cache,
    CoreDbContext db,
    BudgetMapper budgetMapper)
{
    private async Task<Budget> QueryBudgetAsync(CancellationToken ct = default)
    {
        var budgetEntity = await db.Budget.AsNoTracking().FirstOrDefaultAsync(ct);

        if (budgetEntity is null)
            throw new Exception("Budget is not configured");

        return budgetMapper.ToData(budgetEntity);
    }

    public async Task<Budget> GetBudgetAsync()
    {
        var cached = await cache.Budget.TryGetAsync();
        if (cached is not null) return cached;

        var budget = await QueryBudgetAsync();

        await cache.Budget.TrySetAsync(budget);

        return budget;
    }

    public async Task<long> GetAvailableAsync()
    {
        var cached = await cache.BudgetAvailable.TryGetAsync();
        if (cached is not null) return cached.Value;

        var budget = await GetBudgetAsync();

        await cache.BudgetAvailable.TrySetAsync(budget.Available);

        return budget.Available;
    }

    private async Task SetBudgetAsync(long amount)
    {
        var available = await GetAvailableAsync();

        await db.Budget.ExecuteUpdateAsync(setters =>
            setters.SetProperty(b => b.Available, available)
                .SetProperty(b => b.LastSyncAt, DateTime.UtcNow));
    }

    public async Task SyncBudgetAsync()
    {
        var available = await GetAvailableAsync();
        await SetBudgetAsync(available);
    }

    public async Task<DateTime> GetSyncedAtAsync()
    {
        var budget = await QueryBudgetAsync();
        return budget.LastSyncAt;
    }

    public async Task ChangeAvailableAsync(long diff)
    {
        try
        {
            if (cache.IsReady())
            {
                if (diff > 0) await cache.BudgetAvailable.TryIncrementByAsync(diff);
                else await cache.BudgetAvailable.TryDecrementByAsync(Math.Abs(diff));
            }
            else
            {
                await db.Budget.UpdateAsync(x => new BudgetEntity { Available = x.Available + diff });
            }
        }
        catch (Exception)
        {
            logger.LogError("Failed to change available budget");
        }
    }
}