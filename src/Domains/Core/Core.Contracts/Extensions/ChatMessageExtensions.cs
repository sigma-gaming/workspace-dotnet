using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class ChatMessageExtensions
{
    public static ChatMessageData ToData(this ChatMessage entity)
    {
        return new ChatMessageData
        {
            TrackingId = entity.TrackingId,
            CreatedAt = entity.CreatedAt,
            Text = entity.Text,
            Attachments = entity.Attachments,
            SenderName = entity.SenderName,
            SenderUsername = entity.SenderUsername,
            SenderImage = entity.SenderImage,
            UserId = entity.UserId,
            IsPinned = entity.IsPinned,
            ProfileId = entity.ProfileId,
            Id = entity.Id,
            Type = entity.Type,
            SenderRoles = entity.SenderRoles
        };
    }
} 