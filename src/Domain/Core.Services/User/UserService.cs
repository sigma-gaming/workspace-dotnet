using Core.Contracts.Entities;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.User;

public class UserService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper)
{
    private async Task<UserEntity?> QueryUserAsync(Guid userId, CancellationToken ct = default)
    {
        return await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<Contracts.Data.User?> TryGetUserAsync(Guid userId)
    {
        var cached = await cache.User.TryGetAsync(userId);
        if (cached is not null)
            return cached;

        var freshUserEntity = await QueryUserAsync(userId);
        if (freshUserEntity is null)
            return null;

        var user = userMapper.ToData(freshUserEntity);
        await cache.User.TrySetAsync(userId, user);

        return user;
    }

    public async Task<Contracts.Data.User> GetUserAsync(Guid userId)
    {
        var user = await TryGetUserAsync(userId);

        if (user is null)
            throw new Exception("User not found");

        return user;
    }
}