using System.ComponentModel.DataAnnotations;
using Core.Contracts.Data;
using Core.Contracts.Jsonb;
using Core.Services.Auth;
using Core.Services.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Core.API.Controllers;

public sealed record ChatMessagePayload
{
    [StringLength(512, MinimumLength = 1, ErrorMessage = "Слишком длинное сообщение")]
    public string? Text { get; init; }

    [MaxLength(1, ErrorMessage = "Доступно только одно вложение")]
    public ChatMessageAttachment[]? Attachments { get; init; }

    [Required] public required Guid TrackingId { get; init; }
}

[ApiController]
public class ChatController(ChatService chatService) : ControllerBase
{
    [ProducesResponseType<ChatMessage[]>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/chat/last-messages")]
    public async Task<Ok<ChatMessage[]>> GetLastMessages()
    {
        var lastMessages = await chatService.GetLastMessagesAsync();
        return TypedResults.Ok(lastMessages);
    }

    [Authorize]
    [ProducesResponseType<ChatMessage>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/chat/send-message")]
    public async Task<Results<Ok<ChatMessage>, ValidationProblem>> SendMessage(
        [FromBody] ChatMessagePayload payload)
    {
        if (string.IsNullOrEmpty(payload.Text) &&
            (payload.Attachments is null || payload.Attachments.Length == 0))
            return TypedResults.ValidationProblem(
                new Dictionary<string, string[]>
                {
                    [nameof(ChatMessagePayload.Text)] = ["Нельзя отправить пустое сообщение"],
                });

        var userId = HttpContext.GetUserId();

        var message = await chatService.SendMessageAsync(userId, new SendMessagePayload
        {
            Text = payload.Text,
            Attachments = payload.Attachments ?? [],
            TrackingId = payload.TrackingId,
        });

        return TypedResults.Ok(message);
    }
}