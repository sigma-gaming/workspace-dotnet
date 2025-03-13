using System.ComponentModel.DataAnnotations;
using Core.Contracts.Enums;
using Core.Services.Auth;
using Core.Services.PubSub;
using Core.Services.System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Control.API.Controllers;

public sealed record GetMaintenanceStateResponse
{
    public required bool MaintenanceEnabled { get; init; }
    public required bool BackgroundJobsEnabled { get; init; }
}

public sealed record UpdateMaintenanceStatePayload
{
    [Required] public required bool MaintenanceEnabled { get; init; }
    [Required] public required bool BackgroundJobsEnabled { get; init; }
}

public sealed record UpdateMaintenanceStateResponse
{
    public required bool MaintenanceEnabled { get; init; }
    public required bool BackgroundJobsEnabled { get; init; }
}

[ApiController]
public class MaintenanceController(
    MaintenanceService maintenanceService,
    CorePubSubRegistry pubSub) : ControllerBase
{
    [ProducesResponseType<GetMaintenanceStateResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/maintenance/state")]
    public async Task<Ok<GetMaintenanceStateResponse>> GetState()
    {
        var maintenanceEnabled = await maintenanceService.IsMaintenanceEnabledAsync();
        var backgroundJobsEnabled = await maintenanceService.AreBackgroundJobsEnabledAsync();

        return TypedResults.Ok(new GetMaintenanceStateResponse
        {
            MaintenanceEnabled = maintenanceEnabled,
            BackgroundJobsEnabled = backgroundJobsEnabled,
        });
    }

    [AuthorizeRoles(UserRole.Admin)]
    [ProducesResponseType<UpdateMaintenanceStateResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPut("/maintenance/state")]
    public async Task<Results<Ok<UpdateMaintenanceStateResponse>, ValidationProblem>> UpdateState(
        [FromBody] UpdateMaintenanceStatePayload payload)
    {
        var currentMaintenanceEnabled = await maintenanceService.IsMaintenanceEnabledAsync();

        if (payload.MaintenanceEnabled != currentMaintenanceEnabled)
        {
            await maintenanceService.SetMaintenanceEnabledAsync(payload.MaintenanceEnabled);
            if (payload.MaintenanceEnabled) await pubSub.MaintenanceStarted.TryPublishAsync(new { });
        }

        await maintenanceService.SetBackgroundJobsEnabledAsync(payload.BackgroundJobsEnabled);

        return TypedResults.Ok(new UpdateMaintenanceStateResponse
        {
            MaintenanceEnabled = payload.MaintenanceEnabled,
            BackgroundJobsEnabled = payload.BackgroundJobsEnabled,
        });
    }
}