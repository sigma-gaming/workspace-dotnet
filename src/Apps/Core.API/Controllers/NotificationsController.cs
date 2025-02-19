using Core.Contracts.Data;
using Core.Services.Auth;
using Core.Services.Notifications;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

[ApiController]
public class NotificationsController(
    NotificationService notificationService) : ControllerBase
{
    [ProducesResponseType<Notification[]>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/notifications/actual")]
    public async Task<Ok<Notification[]>> GetActual()
    {
        var userId = HttpContext.GetUserId();
        var notifications = await notificationService.GetActualAsync(userId);
        return TypedResults.Ok(notifications);
    }
}