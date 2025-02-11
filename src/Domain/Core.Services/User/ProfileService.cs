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
    public static readonly string[] ReservedUsernames =
    [
        "sigma",
        "sigmadm",
        "sigmaadm",
        "sigmadmin",
        "sigmaadmin",
        "sigmagames",
        "sigmagaming",
        "sigma_admin",
        "sigma_support",
        "sigma_moderator",
        "sigma_developer",
        "sigma_dev",
        "sigma_owner",
        "sigma_bot",
        "sigma_service",
        "sigma_team",
        "sigma_staff",
        "sigma_helpdesk",
        "sigma_system",
        "sigmadev",
        "admin",
        "administrator",
        "moderator",
        "support",
        "helpdesk",
        "system",
        "root",
        "superuser",
        "manager",
        "operator",
        "team",
        "staff",
        "service",
        "developer",
        "dev",
        "owner",
        "bot",
        "api",
        "test",
        "master",
        "security",
        "info",
        "contact",
        "no-reply",
        "notifications",
        "noreply",
        "official",
        "webmaster",
        "help",
        "agent",
        "customer_service",
        "assistant",
        "user_support",
        "community",
        "guide",
        "consultant",
        "agent",
        "moderation",
        "moderators",
        "project",
        "control",
        "account",
        "client_service",
        "sysadmin",
        "team_leader",
        "group_admin",
        "contact_support",
        "customer_care",
        "management",
        "host",
        "monitor",
        "api_service",
        "backend",
        "frontend",
        "engineer",
        "developer_support",
        "server",
        "dashboard",
        "console",
        "admin_panel",
        "config",
        "analytics",
        "update",
        "access",
        "report",
        "auth",
        "authorization",
        "authentication",
    ];

    public static string GetUserFullName(string? firstName, string? lastName)
    {
        return string.Join(" ", new[] { firstName, lastName }
            .Where(name => !string.IsNullOrWhiteSpace(name)));
    }

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

        if (result is null)
            throw new InvalidOperationException("User not found");

        if (result.Profile is null)
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
        if (cached is not null)
            return cached;

        var profile = await QueryUserDetailsAsync(userId);
        await cache.UserDetails.SetAsync(userId, profile);
        return profile;
    }
}