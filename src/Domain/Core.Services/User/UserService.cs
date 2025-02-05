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
        if (!cache.IsReady())
        {
            var userEntity = await QueryUserAsync(userId);
            return userEntity != null ? userMapper.ToData(userEntity) : null;
        }

        var cached = await cache.User.GetAsync(userId);
        if (cached != null)
            return cached;

        var freshUserEntity = await QueryUserAsync(userId);
        if (freshUserEntity == null)
            return null;

        var user = userMapper.ToData(freshUserEntity);
        await cache.User.SetAsync(userId, user);

        return user;
    }

    public async Task<Contracts.Data.User> GetUserAsync(Guid userId)
    {
        var user = await TryGetUserAsync(userId);

        if (user == null)
            throw new Exception("User not found");

        return user;
    }
}