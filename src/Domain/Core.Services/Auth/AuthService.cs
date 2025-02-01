using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Common.Db.Extensions;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Auth;

public enum AuthOutcome
{
    SignedIn,
    SignedUp,
    Connected,
    ConnectedToAnotherUser,
    AnotherAccountConnected
}

public record AuthenticatePayload
{
    public required Guid? UserId { get; init; }
    public required AccountProvider Provider { get; init; }
    public required string ProviderUserId { get; init; }
    public string? ProviderUsername { get; init; }
    public required string ProviderUserFirstName { get; init; }
    public string? ProviderUserLastName { get; init; }
    public string? ProviderUserImage { get; init; }
    public string? ReferralCampaignCode { get; init; }
    public bool? Virtual { get; init; }
}

public record AuthResult
{
    public required AuthOutcome Outcome { get; init; }
    public Contracts.Data.User? User { get; init; }
    public Account? Account { get; init; }

    public static AuthResult SignedIn(Contracts.Data.User user, Account account) =>
        new()
        {
            Outcome = AuthOutcome.SignedIn,
            User = user,
            Account = account
        };

    public static AuthResult SignedUp(Contracts.Data.User user, Account account) =>
        new()
        {
            Outcome = AuthOutcome.SignedUp,
            User = user,
            Account = account
        };

    public static AuthResult Connected() =>
        new()
        {
            Outcome = AuthOutcome.Connected
        };

    public static AuthResult ConnectedToAnotherUser() =>
        new()
        {
            Outcome = AuthOutcome.ConnectedToAnotherUser
        };

    public static AuthResult AnotherAccountConnected() =>
        new()
        {
            Outcome = AuthOutcome.AnotherAccountConnected
        };
}

public class AuthService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    UserService userService,
    AffiliateService affiliateService,
    AccountMapper accountMapper,
    UserMapper userMapper,
    ILogger<AuthService> logger,
    HttpClient httpClient)
{
    private static string GetUserFullName(string firstName, string? lastName) =>
        string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}";

    private async Task<string?> ValidateImageUrlAsync(string imageUrl)
    {
        try
        {
            var response = await httpClient.GetAsync(imageUrl);
            return response.IsSuccessStatusCode ? imageUrl : null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to validate image URL: {ImageUrl}", imageUrl);
            return null;
        }
    }

    public async Task<AuthResult> AuthenticateAsync(AuthenticatePayload payload)
    {
        var account = await db.Accounts
            .AsNoTracking()
            .FirstOrDefaultAsync(a =>
                a.Provider == payload.Provider &&
                a.ProviderUserId == payload.ProviderUserId);

        var accountData = new AccountEntity()
        {
            Provider = payload.Provider,
            ProviderUserId = payload.ProviderUserId,
            ProviderUsername = payload.ProviderUsername,
            ProviderUserFirstName = payload.ProviderUserFirstName,
            ProviderUserLastName = payload.ProviderUserLastName,
            ProviderUserImage = payload.ProviderUserImage != null
                ? await ValidateImageUrlAsync(payload.ProviderUserImage)
                : null
        };

        if (payload.UserId.HasValue)
        {
            var connectedAccount = await db.Accounts
                .AsNoTracking()
                .FirstOrDefaultAsync(a =>
                    a.UserId == payload.UserId &&
                    a.Provider == payload.Provider);

            if (connectedAccount?.ProviderUserId != payload.ProviderUserId)
                return AuthResult.AnotherAccountConnected();

            if (account != null)
            {
                // Account is connected to another user
                if (payload.UserId != account.UserId)
                    return AuthResult.ConnectedToAnotherUser();

                // Account is already connected to current user
                var user = await userService.TryGetUserAsync(payload.UserId.Value);
                if (user == null)
                    throw new InvalidOperationException("User of Account not found");

                return AuthResult.SignedIn(user, accountMapper.ToData(account));
            }

            // Connect new account to existing user
            accountData.UserId = payload.UserId.Value;
            await db.Accounts.AddAsync(accountData);
            await db.SaveChangesAsync();

            await cache.ExecuteIfConnectedAsync(async () =>
                await cache.UserDetails.DeleteAsync(payload.UserId.Value));

            return AuthResult.Connected();
        }

        // Sign in with existing account
        if (account != null)
        {
            var user = await userService.TryGetUserAsync(account.UserId);
            if (user == null)
                throw new InvalidOperationException("User of Account not found");

            return AuthResult.SignedIn(user, accountMapper.ToData(account));
        }

        // Sign up new user
        Guid? referrerId = null;
        Guid? referralCampaignId = null;

        if (!string.IsNullOrEmpty(payload.ReferralCampaignCode))
        {
            var campaign = await affiliateService.TryGetCampaignAsync(payload.ReferralCampaignCode);
            if (campaign != null)
            {
                referrerId = campaign.ReferrerId;
                referralCampaignId = campaign.Id;
            }
        }

        var (createdUserEntity, createdAccountEntity) = await db.ExecuteInTransactionAsync(async () =>
        {
            var createdUserEntity = new UserEntity()
            {
                ReferrerId = referrerId,
                ReferralCampaignId = referralCampaignId,
                Virtual = payload.Virtual ?? false
            };

            await db.Users.AddAsync(createdUserEntity);
            await db.SaveChangesAsync();

            var profile = new ProfileEntity()
            {
                UserId = createdUserEntity.Id,
                UsedProvider = payload.Provider,
                Name = GetUserFullName(payload.ProviderUserFirstName, payload.ProviderUserLastName),
                Image = accountData.ProviderUserImage
            };

            await db.Profiles.AddAsync(profile);
            await db.SaveChangesAsync();

            await db.Balances.AddAsync(new BalanceEntity { UserId = createdUserEntity.Id });

            createdUserEntity.ProfileId = profile.Id;
            db.Users.Update(createdUserEntity);

            accountData.UserId = createdUserEntity.Id;
            var createdAccount = await db.Accounts.AddAsync(accountData);

            await db.UserSecurities.AddAsync(new UserSecurityEntity { UserId = createdUserEntity.Id });

            if (referralCampaignId.HasValue)
                await affiliateService.IncrementCampaignSignupsAsync(referralCampaignId.Value);

            await db.SaveChangesAsync();
            return (createdUserEntity, createdAccount.Entity);
        });

        var createdUser = userMapper.ToData(createdUserEntity);

        await cache.ExecuteIfConnectedAsync(
            async () => await cache.User.SetAsync(createdUserEntity.Id, createdUser));

        return AuthResult.SignedUp(createdUser, accountMapper.ToData(createdAccountEntity));
    }
}