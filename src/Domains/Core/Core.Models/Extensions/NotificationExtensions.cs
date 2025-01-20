using Core.Domain.Entities;
using Core.Models.Data;
using Core.Models.Entities;

namespace Core.Domain.Entities;

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