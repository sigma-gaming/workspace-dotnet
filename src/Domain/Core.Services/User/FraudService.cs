using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.User;

public class FraudService(
    CoreDbContext db,
    ILogger<FraudService> logger)
{
    private static readonly ScoreConfig[] ScoreConfigs =
    [
        new(nameof(UserSecurityEntity.LastIp), nameof(UserSecurityEntity.IpScore), s => s.LastIp,
            s => s.IpScore, 15),
        new(nameof(UserSecurityEntity.AddressTon), nameof(UserSecurityEntity.AddressTonScore), s => s.AddressTon,
            s => s.AddressTonScore, 5),
        new(nameof(UserSecurityEntity.AddressUsdt), nameof(UserSecurityEntity.AddressUsdtScore), s => s.AddressUsdt,
            s => s.AddressUsdtScore, 5),
        new(nameof(UserSecurityEntity.AddressUsdc), nameof(UserSecurityEntity.AddressUsdcScore), s => s.AddressUsdc,
            s => s.AddressUsdcScore, 5),
        new(nameof(UserSecurityEntity.AddressTrx), nameof(UserSecurityEntity.AddressTrxScore), s => s.AddressTrx,
            s => s.AddressTrxScore, 5),
        new(nameof(UserSecurityEntity.AddressEth), nameof(UserSecurityEntity.AddressEthScore), s => s.AddressEth,
            s => s.AddressEthScore, 5),
        new(nameof(UserSecurityEntity.AddressBtc), nameof(UserSecurityEntity.AddressBtcScore), s => s.AddressBtc,
            s => s.AddressBtcScore, 5),
        new(nameof(UserSecurityEntity.AddressLtc), nameof(UserSecurityEntity.AddressLtcScore), s => s.AddressLtc,
            s => s.AddressLtcScore, 5),
        new(nameof(UserSecurityEntity.AddressDoge), nameof(UserSecurityEntity.AddressDogeScore), s => s.AddressDoge,
            s => s.AddressDogeScore, 5),
        new(nameof(UserSecurityEntity.AddressBnb), nameof(UserSecurityEntity.AddressBnbScore), s => s.AddressBnb,
            s => s.AddressBnbScore, 5),
        new(nameof(UserSecurityEntity.AddressXmr), nameof(UserSecurityEntity.AddressXmrScore), s => s.AddressXmr,
            s => s.AddressXmrScore, 5),
        new(nameof(UserSecurityEntity.AddressSol), nameof(UserSecurityEntity.AddressSolScore), s => s.AddressSol,
            s => s.AddressSolScore, 5),
    ];

    private static FraudRisk CalculateRisk(int score)
    {
        return score switch
        {
            >= 75 => FraudRisk.High,
            >= 45 => FraudRisk.Medium,
            >= 30 => FraudRisk.Low,
            _ => FraudRisk.Clear,
        };
    }

    public async Task WhitelistUserAsync(Guid userId)
    {
        await db.UserSecurities
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(u => u.Whitelisted, true));
    }

    public async Task<FraudRisk> ActualizeRiskAsync(Guid userId, string? ip = null, bool updateIp = true)
    {
        try
        {
            var security = await db.UserSecurities
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (security is null)
            {
                logger.LogWarning("Failed to get UserSecurity entity for user {UserId}", userId);
                return FraudRisk.Clear;
            }

            if (security.Whitelisted)
                return FraudRisk.Clear;

            var lastIp = security.LastIp;

            if (string.IsNullOrEmpty(lastIp) || updateIp)
            {
                logger.LogInformation("Updating IP for user {UserId}", userId);
                lastIp = ip;

                if (!string.IsNullOrEmpty(lastIp))
                {
                    security.LastIp = lastIp;
                    await db.SaveChangesAsync();
                }
                else
                {
                    logger.LogWarning("Failed to get IP. Probably context is not specified");
                }
            }

            var totalScore = 0;

            foreach (var config in ScoreConfigs)
            {
                var value = config.FieldSelector(security);

                if (string.IsNullOrEmpty(value))
                    continue;

                var sameCount = await db.UserSecurities
                    .CountAsync(s =>
                        EF.Property<string>(s, config.FieldName) == value);

                var newScore = sameCount * config.Multiplier;

                logger.LogInformation("Updating {ScoreField} for {Count} users with the same {Field}",
                    config.ScoreSelector.ToString(), sameCount, config.FieldSelector.ToString());

                await db.UserSecurities
                    .Where(s => EF.Property<string>(s, config.FieldName) == value &&
                                EF.Property<int>(s, config.ScoreName) < newScore)
                    .ExecuteUpdateAsync(setters =>
                        // ReSharper disable once EntityFramework.ClientSideDbFunctionCall
                        setters.SetProperty(s => EF.Property<int>(s, config.ScoreName), newScore));

                totalScore += newScore;
            }

            return CalculateRisk(totalScore);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to actualize risk");
            return FraudRisk.Unknown;
        }
    }

    private record ScoreConfig(
        string FieldName,
        string ScoreName,
        Func<UserSecurityEntity, string?> FieldSelector,
        Func<UserSecurityEntity, int> ScoreSelector,
        int Multiplier);
}