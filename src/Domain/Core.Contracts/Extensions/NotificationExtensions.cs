using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class NotificationExtensions
{
    public static Notification ToData(this NotificationEntity entity)
    {
        return new Notification
        {
            Kind = entity.Kind,
            Id = entity.Id,
            CreatedAt = entity.CreatedAt,
            ExpiresAt = entity.ExpiresAt,
            Title = entity.Title,
            Message = entity.Message,
            AutoClose = entity.AutoClose,
            AutoCloseMs = entity.AutoCloseMs,
            WithCloseButton = entity.WithCloseButton,
            UserId = entity.UserId
        };
    }
} 