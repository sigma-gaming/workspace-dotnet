using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.PubSub;
using Core.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Chat;

public class ChatService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    ProfileService profileService,
    ChatMessageMapper mapper,
    ILogger<ChatService> logger)
{
    public async Task InitializeMessagesAsync()
    {
        if (!cache.IsReady())
        {
            logger.LogWarning("Cannot initialize chat messages, cache is not ready");
            return;
        }

        await cache.ExecuteIfConnectedAsync(async () =>
        {
            var exists = await cache.LastChatMessages.ExistsAsync();

            if (exists) return;

            var messages = await db.ChatMessages
                .OrderByDescending(m => m.Id)
                .Take(50)
                .ToListAsync();

            await cache.LastChatMessages.SetAllAsync(messages.Select(mapper.ToData).ToList());
        });
    }

    public async Task<ChatMessage[]> GetLastMessagesAsync()
    {
        if (!cache.IsReady())
        {
            var messages = await db.ChatMessages
                .OrderByDescending(m => m.Id)
                .Take(50)
                .ToArrayAsync();

            return messages.Select(mapper.ToData).ToArray();
        }

        var saved = await cache.LastChatMessages.GetAllAsync();
        return saved.ToArray();
    }

    public async Task<ChatMessage> SendMessageAsync(
        Guid? userId,
        SendMessagePayload payload)
    {
        var messageInsert = new ChatMessageEntity
        {
            Type = userId.HasValue ? ChatMessageType.UserMessage : ChatMessageType.SystemMessage,
            Text = payload.Text,
            Attachments = payload.Attachments,
            TrackingId = payload.TrackingId,
        };

        if (userId.HasValue)
        {
            var userDetails = await profileService.GetUserDetailsAsync(userId.Value);

            messageInsert.UserId = userId;
            messageInsert.ProfileId = userDetails.Profile.Id;
            messageInsert.SenderName = userDetails.Profile.Name;
            messageInsert.SenderUsername = userDetails.Profile.Username;
            messageInsert.SenderImage = userDetails.Profile.Image;
            messageInsert.SenderRoles = userDetails.User.Roles;
        }
        else
        {
            messageInsert.SenderRoles = new[] { UserRole.Admin };
        }

        var entry = await db.ChatMessages.AddAsync(messageInsert);
        await db.SaveChangesAsync();

        var message = mapper.ToData(entry.Entity);

        await cache.ExecuteIfConnectedAsync(async () => { await cache.LastChatMessages.LeftPushAsync(message); });

        await pubSub.ChatMessages.PublishAsync(message);

        return message;
    }

    // private async Task ValidateGameAttachmentAsync(Guid userId, ChatMessageAttachmentGame attachment)
    // {
    //     var gameRecord = await db.GameRecords
    //         .Where(g => g.Id == attachment.GameRecordId)
    //         .FirstOrDefaultAsync();
    //
    //     if (gameRecord is null) throw new Exception("Game record not found");
    //
    //     if (gameRecord.UserId != userId) throw new BadHttpRequestException("Нельзя отправлять чужие игры");
    // }
    //
    // private async Task ValidateAttachmentAsync(Guid userId, ChatMessageAttachment attachment)
    // {
    //     if (attachment is ChatMessageAttachmentGame gameAttachment)
    //     {
    //         await ValidateGameAttachmentAsync(userId, gameAttachment);
    //         return;
    //     }
    //
    //     throw new BadHttpRequestException("Недопустимое вложение");
    // }
}

public record SendMessagePayload
{
    public string? Text { get; init; }
    public ChatMessageAttachment[] Attachments { get; init; } = [];
    public Guid TrackingId { get; init; }
}