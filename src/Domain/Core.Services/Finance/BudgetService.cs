using Core.Contracts.Data;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        if (budgetEntity == null)
            throw new Exception("Budget is not configured");

        return budgetMapper.ToData(budgetEntity);
    }

    public async Task<Budget> GetBudgetAsync()
    {
        if (!cache.IsReady()) return await QueryBudgetAsync();

        var cached = await cache.Budget.GetAsync();
        if (cached != null) return cached;

        var budget = await QueryBudgetAsync();
        await cache.Budget.SetAsync(budget);

        return budget;
    }

    public async Task<long> GetAvailableAsync()
    {
        if (!cache.IsReady()) return (await QueryBudgetAsync()).Available;

        var cached = await cache.BudgetAvailable.GetAsync();
        if (cached != null) return cached.Value;

        var budget = await GetBudgetAsync();
        await cache.BudgetAvailable.SetAsync(budget.Available);

        return budget.Available;
    }

    public async Task SyncBudgetAsync()
    {
        var available = await GetAvailableAsync();

        await db.Budget.ExecuteUpdateAsync(setters =>
            setters.SetProperty(b => b.Available, available));
    }

    public async Task<DateTime> GetSyncedAtAsync()
    {
        var budget = await QueryBudgetAsync();
        return budget.LastSyncAt;
    }

    public async Task IncreaseAvailableAsync(long amount)
    {
        try
        {
            await cache.BudgetAvailable.IncrementByAsync(amount);
        }
        catch (Exception)
        {
            logger.LogError("Failed to increase available budget");
        }
    }

    public async Task DecreaseAvailableAsync(long amount)
    {
        try
        {
            await cache.BudgetAvailable.DecrementByAsync(amount);
        }
        catch (Exception)
        {
            logger.LogError("Failed to decrease available budget");
        }
    }

    public async Task ChangeAvailableAsync(long diff)
    {
        if (diff > 0) await IncreaseAvailableAsync(diff);
        else await DecreaseAvailableAsync(Math.Abs(diff));
    }
}