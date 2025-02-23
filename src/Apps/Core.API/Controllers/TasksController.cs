using System.ComponentModel.DataAnnotations;
using Core.API.Infrastructure.RateLimiting;
using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Services.Auth;
using Core.Services.Promotions;
using Core.Services.Promotions.TaskCheckers;
using Core.Services.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.API.Controllers;

public sealed record CompleteTaskPayload
{
    [Required] public required GlobalTaskKey TaskKey { get; init; }
}

public sealed record CompleteTaskResponse
{
    public required UserUpdate<GlobalTaskStatus> Task { get; init; }
}

public sealed record ClaimRewardResponse
{
    public required long Payout { get; init; }
    public required UserUpdate<GlobalTaskStatus> Task { get; init; }
    public required UserUpdate<BalancePublic> Balance { get; init; }
}

[ApiController]
public class TasksController(
    CorePubSubRegistry pubSub,
    GlobalTaskService globalTaskService,
    GlobalTaskCheckerProvider checkerProvider,
    BalanceMapper balanceMapper) : ControllerBase
{
    [Authorize]
    [ProducesResponseType<Dictionary<GlobalTaskKey, TaskStatus>>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/tasks/global/my-statuses")]
    public async Task<Ok<Dictionary<GlobalTaskKey, TaskStatus>>> GetGlobalTaskStatuses()
    {
        var userId = HttpContext.GetUserId();
        var statuses = new Dictionary<GlobalTaskKey, TaskStatus>();

        foreach (var taskKey in Enum.GetValues<GlobalTaskKey>())
        {
            var status = await globalTaskService.GetStatusAsync(taskKey, userId);
            statuses[taskKey] = status.Status;
        }

        return TypedResults.Ok(statuses);
    }

    [ProducesResponseType<GlobalTask[]>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/tasks/global/list")]
    public async Task<Ok<GlobalTask[]>> ListGlobalTasks()
    {
        var tasks = await globalTaskService.GetTasksAsync();
        return TypedResults.Ok(tasks);
    }

    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.Task.Action)]
    [ProducesResponseType<CompleteTaskResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/tasks/global/complete")]
    public async Task<Results<Ok<CompleteTaskResponse>, ValidationProblem>> CompleteGlobalTask(
        [FromBody] CompleteTaskPayload payload)
    {
        var userId = HttpContext.GetUserId();
        var checker = checkerProvider.GetChecker(payload.TaskKey);

        if (checker == null)
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(CompleteTaskPayload.TaskKey)] = ["Неподдерживаемый тип задания"],
                });

        var completion = await globalTaskService.CompleteTaskAsync(
            userId,
            payload.TaskKey,
            checker
        );

        if (completion.Outcome != GlobalTaskCompleteOutcome.Completed)
        {
            var message = completion.Outcome switch
            {
                GlobalTaskCompleteOutcome.AlreadyCompleted => "Задание уже выполнено",
                GlobalTaskCompleteOutcome.NotCompleted => completion.Message ?? "Задание еще не выполнено",
                GlobalTaskCompleteOutcome.NotActive => "Задание не активно",
                _ => "Не удалось проверить задание. Попробуйте еще раз или напишите в поддержку",
            };

            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(CompleteTaskPayload.TaskKey)] = [message],
                });
        }

        var task = new UserUpdate<GlobalTaskStatus>
        {
            UserId = userId,
            Mode = UpdateMode.Optimized,
            Data = new GlobalTaskStatus
            {
                Status = completion.UpdatedStatus!.Value,
                TaskKey = payload.TaskKey,
                UserId = userId,
            },
        };

        await pubSub.GlobalTaskStatusUpdated.TryPublishAsync(task);

        return TypedResults.Ok(new CompleteTaskResponse { Task = task });
    }

    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.Task.Action)]
    [ProducesResponseType<ClaimRewardResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/tasks/global/claim-reward")]
    public async Task<Results<Ok<ClaimRewardResponse>, ValidationProblem>> ClaimGlobalTaskReward(
        [FromBody] CompleteTaskPayload payload)
    {
        var userId = HttpContext.GetUserId();

        var claim = await globalTaskService.ClaimRewardAsync(userId, payload.TaskKey);

        if (claim.Outcome != GlobalTaskClaimRewardOutcome.Claimed)
        {
            var message = claim.Outcome switch
            {
                GlobalTaskClaimRewardOutcome.AlreadyClaimed => "Награда уже получена",
                GlobalTaskClaimRewardOutcome.NotCompleted => "Задание еще не выполнено",
                _ => "Не удалось получить награду. Попробуйте еще раз или напишите в поддержку",
            };

            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(CompleteTaskPayload.TaskKey)] = [message],
                });
        }

        var task = new UserUpdate<GlobalTaskStatus>
        {
            UserId = userId,
            Mode = UpdateMode.Optimized,
            Data = new GlobalTaskStatus
            {
                Status = claim.UpdatedStatus!.Value,
                TaskKey = payload.TaskKey,
                UserId = userId,
            },
        };

        var balance = new UserUpdate<BalancePublic>
        {
            UserId = userId,
            Mode = UpdateMode.Optimized,
            Data = balanceMapper.ToPublic(claim.UpdatedBalance!),
        };

        await pubSub.BalanceUpdated.TryPublishAsync(balance);
        await pubSub.GlobalTaskStatusUpdated.TryPublishAsync(task);

        return TypedResults.Ok(new ClaimRewardResponse
        {
            Payout = claim.Payout,
            Task = task,
            Balance = balance,
        });
    }
}