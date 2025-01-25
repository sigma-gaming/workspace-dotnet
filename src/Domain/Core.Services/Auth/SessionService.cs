using Core.Contracts.Data;
using Core.Contracts.Mappers;
using Core.Services.Caching;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Auth;

public class SessionService(
    CoreCacheRegistry cache,
    CoreDbContext db,
    SessionMapper sessionMapper)
{
    public async Task<Session?> GetSessionAsync(Guid sessionId)
    {
        var cachedSession = await cache.Session.GetAsync(sessionId);

        if (cachedSession != null)
        {
            return cachedSession.ExpiresAt > DateTime.UtcNow ? cachedSession : null;
        }

        var sessionEntity = await db.Sessions
            .FirstOrDefaultAsync(s => s.Id == sessionId);

        if (sessionEntity == null)
            return null;

        var session = sessionMapper.ToData(sessionEntity);
        if (session.ExpiresAt <= DateTime.UtcNow)
            return null;

        await cache.Session.SetAsync(session.Id, session);

        return session;
    }
}