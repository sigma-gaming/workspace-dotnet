using Core.Contracts.Compounds;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.User;

public class ProfileService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserMapper userMapper,
    ProfileMapper profileMapper,
    AccountMapper accountMapper)
{
    private async Task<UserDetails> QueryUserDetailsAsync(Guid userId, CancellationToken ct = default)
    {
        var result = await (
                from user in db.Users
                join p in db.Profiles on user.Id equals p.UserId into userProfiles
                from profile in userProfiles.DefaultIfEmpty()
                join a in db.Accounts on user.Id equals a.UserId into userAccounts
                where user.Id == userId
                select new
                {
                    User = user,
                    Profile = profile,
                    Accounts = userAccounts.ToList(),
                }
            )
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (result == null)
            throw new InvalidOperationException("User not found");

        if (result.Profile == null)
            throw new InvalidOperationException("Profile not found");

        return new UserDetails
        {
            User = userMapper.ToPublic(result.User),
            Profile = profileMapper.ToData(result.Profile),
            Accounts = result.Accounts.Select(accountMapper.ToData).ToArray(),
        };
    }

    public async Task<UserDetails> GetUserDetailsAsync(Guid userId)
    {
        if (!cache.IsReady())
            return await QueryUserDetailsAsync(userId);

        var cached = await cache.UserDetails.GetAsync(userId);
        if (cached != null)
            return cached;

        var profile = await QueryUserDetailsAsync(userId);
        await cache.UserDetails.SetAsync(userId, profile);
        return profile;
    }
}