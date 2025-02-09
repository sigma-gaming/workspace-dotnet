using Common.Db.Extensions;
using Core.Contracts.Compounds;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.Services.Promotions;

public enum GlobalTaskCompleteOutcome
{
    AlreadyCompleted,
    NotCompleted,
    Completed,
    NotActive,
    Failed,
}

public enum GlobalTaskClaimRewardOutcome
{
    AlreadyClaimed,
    NotCompleted,
    Claimed,
    Failed,
}

public sealed record GlobalTaskCompleteOutput(
    GlobalTaskCompleteOutcome Outcome,
    TaskStatus? UpdatedStatus = null,
    string? Message = null);

public sealed record GlobalTaskClaimRewardOutput(
    GlobalTaskClaimRewardOutcome Outcome,
    long Payout = 0,
    TaskStatus? UpdatedStatus = null,
    BalanceEntity? UpdatedBalance = null);

public sealed record GlobalTaskCheckResult(
    bool Completed,
    string? Message = null);

public class GlobalTaskService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ILogger<GlobalTaskService> logger,
    BalanceService balanceService,
    GlobalTaskMapper taskMapper,
    BalanceMapper balanceMapper)
{
    private async Task<GlobalTask[]> QueryTasksAsync()
    {
        return await db.GlobalTasks
            .Select(t => taskMapper.ToData(t))
            .AsNoTracking()
            .ToArrayAsync();
    }

    public async Task<GlobalTask[]> GetTasksAsync()
    {
        if (!cache.IsReady()) return await QueryTasksAsync();

        var cached = await cache.GlobalTasks.GetAsync();
        if (cached != null) return cached;

        var tasks = await QueryTasksAsync();
        await cache.GlobalTasks.SetAsync(tasks);
        return tasks;
    }

    public async Task<GlobalTask?> GetTaskAsync(GlobalTaskKey taskKey)
    {
        var tasks = await GetTasksAsync();
        return tasks.FirstOrDefault(t => t.Key == taskKey);
    }

    private async Task<GlobalTaskStatusInfo> QueryStatusAsync(GlobalTaskKey taskKey, Guid userId)
    {
        var status = await db.GlobalTaskStatuses
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.TaskKey == taskKey && s.UserId == userId);

        return status != null
            ? new GlobalTaskStatusInfo(status.Status, true)
            : new GlobalTaskStatusInfo(TaskStatus.Pending, false);
    }

    public async Task<GlobalTaskStatusInfo> GetStatusAsync(GlobalTaskKey taskKey, Guid userId)
    {
        if (!cache.IsReady()) return await QueryStatusAsync(taskKey, userId);

        var cached = await cache.GlobalTaskStatus.GetAsync($"{taskKey}:{userId}");
        if (cached != null) return cached;

        var status = await QueryStatusAsync(taskKey, userId);
        await cache.GlobalTaskStatus.SetAsync($"{taskKey}:{userId}", status);
        return status;
    }

    public async Task<GlobalTaskStatusEntity?> GetStatusForUpdateAsync(GlobalTaskKey taskKey, Guid userId)
    {
        return await db.GlobalTaskStatuses
            .ForUpdate()
            .FirstOrDefaultAsync(s => s.TaskKey == taskKey && s.UserId == userId);
    }

    public async Task UpdateStatusAsync(GlobalTaskKey taskKey, Guid userId, TaskStatus status)
    {
        var info = await GetStatusAsync(taskKey, userId);

        if (info.IsInserted)
            await db.GlobalTaskStatuses
                .AsNoTracking()
                .Where(s => s.TaskKey == taskKey && s.UserId == userId)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(s => s.Status, status));
        else
            await db.GlobalTaskStatuses.AddAsync(new GlobalTaskStatusEntity
            {
                TaskKey = taskKey,
                UserId = userId,
                Status = status,
            });
    }

    public async Task<GlobalTaskCompleteOutput> CompleteTaskAsync(
        Guid userId,
        GlobalTaskKey taskKey,
        Func<Guid, GlobalTask, Task<GlobalTaskCheckResult>> checker)
    {
        try
        {
            var output = await db.ExecuteInTransactionAsync(async () =>
            {
                var statusEntity = await GetStatusForUpdateAsync(taskKey, userId);

                var status = statusEntity?.Status ?? TaskStatus.Pending;

                if (status != TaskStatus.Pending)
                    return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.AlreadyCompleted);

                var task = await GetTaskAsync(taskKey);

                if (task == null)
                    return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.Failed);

                if (!task.IsActive)
                    return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.NotActive);

                var checkResult = await checker(userId, task);

                if (!checkResult.Completed)
                    return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.NotCompleted,
                        Message: checkResult.Message);

                await UpdateStatusAsync(taskKey, userId, TaskStatus.Completed);

                return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.Completed,
                    TaskStatus.Completed);
            });

            await cache.ExecuteIfConnectedAsync(async () =>
            {
                if (output.UpdatedStatus != null)
                    await cache.GlobalTaskStatus.SetAsync($"{taskKey}:{userId}",
                        new GlobalTaskStatusInfo(output.UpdatedStatus.Value, true));
            });

            return output;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to complete task");
            return new GlobalTaskCompleteOutput(GlobalTaskCompleteOutcome.Failed);
        }
    }

    public async Task<GlobalTaskClaimRewardOutput> ClaimRewardAsync(Guid userId, GlobalTaskKey taskKey)
    {
        try
        {
            var output = await db.ExecuteInTransactionAsync(async () =>
            {
                var statusEntity = await GetStatusForUpdateAsync(taskKey, userId);

                var status = statusEntity?.Status ?? TaskStatus.Pending;

                if (status == TaskStatus.Claimed)
                    return new GlobalTaskClaimRewardOutput(GlobalTaskClaimRewardOutcome.AlreadyClaimed);

                if (status != TaskStatus.Completed)
                    return new GlobalTaskClaimRewardOutput(GlobalTaskClaimRewardOutcome.NotCompleted);

                var task = await db.GlobalTasks
                    .FirstOrDefaultAsync(t => t.Key == taskKey);

                if (task == null)
                    return new GlobalTaskClaimRewardOutput(GlobalTaskClaimRewardOutcome.Failed);

                var balanceEntity = await balanceService.GetBalanceForUpdateAsync(userId);

                var wageringChange = (long)Math.Ceiling(task.Payout * (task.WageringMultiplier / 100.0));

                var transaction = await balanceService.CreateTransactionAsync(new TransactionEntity
                {
                    UserId = userId,
                    Type = TransactionType.Bonus,
                    Amount = task.Payout,
                });

                var updatedBalance = await balanceService.UpdateBalanceAsync(
                    balanceEntity,
                    transaction,
                    null,
                    wageringChange
                );

                await UpdateStatusAsync(taskKey, userId, TaskStatus.Claimed);

                return new GlobalTaskClaimRewardOutput(
                    GlobalTaskClaimRewardOutcome.Claimed,
                    task.Payout,
                    TaskStatus.Claimed,
                    updatedBalance);
            });

            await cache.ExecuteIfConnectedAsync(async () =>
            {
                if (output.UpdatedStatus != null)
                    await cache.GlobalTaskStatus.SetAsync($"{taskKey}:{userId}",
                        new GlobalTaskStatusInfo(output.UpdatedStatus.Value, true));

                if (output.UpdatedBalance != null)
                    await cache.UserBalance.SetAsync(userId, balanceMapper.ToData(output.UpdatedBalance));
            });

            return output;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to claim reward");
            return new GlobalTaskClaimRewardOutput(GlobalTaskClaimRewardOutcome.Failed);
        }
    }
}