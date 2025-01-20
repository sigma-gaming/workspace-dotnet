using Core.Contracts.Data;
using Core.Contracts.Entities;

namespace Core.Contracts.Extensions;

public static class NotificationExtensions
{
    public static NotificationData ToData(this Notification entity)
    {
        return new NotificationData
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