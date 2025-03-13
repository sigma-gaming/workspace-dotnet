using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.User;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Auth;

public sealed record CreateSessionPayload
{
    public required Guid UserId { get; init; }
    public Guid? ReferrerId { get; init; }
    public Guid? ReferralCampaignId { get; init; }
    public AccountProvider Provider { get; init; }
}

public class SessionService(
    CoreCacheRegistry cache,
    CoreDbContext db,
    UserService userService,
    SessionMapper sessionMapper)
{
    public async Task<Session?> GetSessionAsync(Guid sessionId)
    {
        var cachedSession = await cache.Session.TryGetAsync(sessionId);

        if (cachedSession is not null)
            return cachedSession.ExpiresAt > DateTime.UtcNow ? cachedSession : null;

        var sessionEntity = await db.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sessionEntity is null)
            return null;

        var session = sessionMapper.ToData(sessionEntity);
        if (session.ExpiresAt <= DateTime.UtcNow)
            return null;

        await cache.Session.TrySetAsync(session.Id, session);

        return session;
    }

    public async Task<SessionEntity> CreateSession(CreateSessionPayload payload)
    {
        var expiresAt = DateTime.UtcNow.AddDays(31);

        var user = await userService.GetUserAsync(payload.UserId);
        if (user is null) throw new InvalidOperationException($"User with ID {payload.UserId} not found");

        var sessionEntity = new SessionEntity
        {
            Id = Guid.NewGuid(),
            UserId = payload.UserId,
            ReferrerId = payload.ReferrerId,
            ReferralCampaignId = payload.ReferralCampaignId,
            Provider = payload.Provider,
            Roles = user.Roles,
            ExpiresAt = expiresAt,
        };

        db.Sessions.Add(sessionEntity);
        await db.SaveChangesAsync();

        var session = sessionMapper.ToData(sessionEntity);
        await cache.Session.TrySetAsync(session.Id, session);

        await db.Sessions
            .Where(s => s.UserId == payload.UserId)
            .OrderByDescending(s => s.ExpiresAt)
            .Skip(5)
            .ExecuteDeleteAsync();

        return sessionEntity;
    }

    public async Task<DateTime> RefreshSessionAsync(Guid sessionId)
    {
        var expiresAt = DateTime.UtcNow.AddMonths(1);

        var rows = await db.Sessions.Where(s => s.Id == sessionId)
            .ExecuteUpdateAsync(update =>
                update.SetProperty(s => s.ExpiresAt, expiresAt));

        if (rows == 0)
            throw new InvalidOperationException("Session not found");

        return expiresAt;
    }

    public async Task<bool> RemoveSessionAsync(Guid sessionId)
    {
        var rows = await db.Sessions.Where(s => s.Id == sessionId)
            .ExecuteDeleteAsync();

        return rows == 1;
    }
}