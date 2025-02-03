using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.PubSub;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Notifications;

public class NotificationService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    NotificationMapper mapper)
{
    private async Task<Notification[]> QueryPersonalAsync(Guid userId)
    {
        var now = DateTime.UtcNow;

        var notifications = await db.Notifications
            .Where(n => n.UserId == userId && n.ExpiresAt >= now)
            .OrderBy(n => n.CreatedAt)
            .AsNoTracking()
            .ToArrayAsync();
        
        return notifications.Select(mapper.ToData).ToArray();
    }

    private async Task<Notification[]> QueryGlobalAsync()
    {
        var now = DateTime.UtcNow;

        var notifications = await db.Notifications
            .Where(n => n.UserId == null && n.ExpiresAt > now)
            .OrderBy(n => n.CreatedAt)
            .AsNoTracking()
            .ToArrayAsync();

        return notifications.Select(mapper.ToData).ToArray();
    }

    private async Task<Notification[]> GetPersonalAsync(Guid userId)
    {
        if (!cache.IsReady())
            return await QueryPersonalAsync(userId);

        var cached = await cache.PersonalNotifications.GetAsync(userId);
        if (cached is not null)
            return cached;

        var personal = await QueryPersonalAsync(userId);
        await cache.PersonalNotifications.SetAsync(userId, personal);
        return personal;
    }

    private async Task<Notification[]> GetGlobalAsync()
    {
        if (!cache.IsReady())
            return await QueryGlobalAsync();

        var cached = await cache.GlobalNotifications.GetAsync();
        if (cached is not null)
            return cached;

        var global = await QueryGlobalAsync();
        await cache.GlobalNotifications.SetAsync(global);
        return global;
    }

    public async Task<Notification[]> GetActualAsync(Guid? userId = null)
    {
        var notifications = new List<Notification>();

        if (userId.HasValue)
        {
            var personal = await GetPersonalAsync(userId.Value);
            notifications.AddRange(personal);
        }

        var global = await GetGlobalAsync();
        notifications.AddRange(global);
        return notifications.ToArray();
    }

    public async Task<Notification> SendAsync(NotificationEntity notification)
    {
        await db.Notifications.AddAsync(notification);
        await db.SaveChangesAsync();

        var result = mapper.ToData(notification);

        if (!cache.IsReady())
            return result;

        if (notification.UserId.HasValue)
            await cache.PersonalNotifications.DeleteAsync(notification.UserId.Value);
        else
            await cache.GlobalNotifications.DeleteAsync();

        await pubSub.Notifications.PublishAsync(result);
        return result;
    }
}