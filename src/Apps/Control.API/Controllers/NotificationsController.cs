using System.ComponentModel.DataAnnotations;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Services.Auth;
using Core.Services.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Control.API.Controllers;

public sealed record SendNotificationPayload
{
    [Required] public required DateTime ExpiresAt { get; init; }
    [Required] public required string Title { get; init; }
    [Required] public required string Message { get; init; }
    [Required] public required NotificationKind Kind { get; init; }
    [Required] public required bool AutoClose { get; init; }
    [Required] public required int AutoCloseMs { get; init; }
    [Required] public required bool WithCloseButton { get; init; }
    public Guid? UserId { get; init; }
}

public sealed record SendNotificationResponse
{
    public required string Status { get; init; }
}

[ApiController]
public class NotificationsController(
    NotificationService notificationService) : ControllerBase
{
    [ProducesResponseType<SendNotificationResponse>(StatusCodes.Status200OK, "application/json")]
    [AuthorizeRoles(UserRole.Admin)]
    [HttpPost("/notifications/send")]
    public async Task<Ok<SendNotificationResponse>> Send([FromBody] SendNotificationPayload payload)
    {
        var notificationEntity = new NotificationEntity
        {
            ExpiresAt = payload.ExpiresAt,
            Title = payload.Title,
            Message = payload.Message,
            Kind = payload.Kind,
            AutoClose = payload.AutoClose,
            AutoCloseMs = payload.AutoCloseMs,
            WithCloseButton = payload.WithCloseButton,
            UserId = payload.UserId,
        };

        await notificationService.SendAsync(notificationEntity);
        return TypedResults.Ok(new SendNotificationResponse { Status = "success" });
    }

    [ProducesResponseType<Notification[]>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/notifications/actual")]
    public async Task<Ok<Notification[]>> GetActual()
    {
        var userId = HttpContext.GetUserId();
        var notifications = await notificationService.GetActualAsync(userId);
        return TypedResults.Ok(notifications);
    }
}