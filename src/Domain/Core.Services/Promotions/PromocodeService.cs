using Common.Db.Extensions;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Promotions;

public abstract record PromocodeActivationResult
{
    public required PromocodeActivationOutcome Outcome { get; init; }
}

public sealed record PromocodeActivationPayoutResult : PromocodeActivationResult
{
    public required long Payout { get; init; }
    public required BalanceEntity UpdatedBalance { get; init; }
    public required PromocodeEntity UpdatedPromocode { get; init; }
    public required long WageringRequired { get; init; }
}

public sealed record PromocodeActivationDepositResult : PromocodeActivationResult
{
    public required long Payout { get; init; }
    public required long WageringRequired { get; init; }
}

public sealed record PromocodeActivationWrongUsageResult : PromocodeActivationResult
{
    public required PromocodeBonusType BonusType { get; init; }
}

public sealed record PromocodeActivationFailedResult : PromocodeActivationResult;

public class PromocodeService(
    ILogger<PromocodeService> logger,
    CoreDbContext db,
    CoreCacheRegistry cache,
    FraudService fraudService,
    PromocodeMapper promocodeMapper,
    BalanceMapper balanceMapper)
{
    private const string DefaultAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public string GenerateOne(int length = 10, string alphabet = DefaultAlphabet)
    {
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => alphabet[random.Next(alphabet.Length)])
            .ToArray());
    }

    public string[] GenerateMany(int count, int length = 10, string alphabet = DefaultAlphabet)
    {
        return Enumerable.Range(0, count)
            .Select(_ => GenerateOne(length, alphabet))
            .ToArray();
    }

    public async Task<Promocode?> GetPromocodeAsync(string code)
    {
        if (cache.IsReady())
        {
            var cached = await cache.Promocode.GetAsync(code);
            if (cached != null) return cached;
        }

        var promocodeEntity = await db.Promocodes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Code == code);

        if (promocodeEntity == null) return null;

        var promocode = promocodeMapper.ToData(promocodeEntity);

        await cache.ExecuteIfConnectedAsync(async () => { await cache.Promocode.SetAsync(code, promocode); });

        return promocode;
    }

    public async Task<bool> IsUsedAsync(Guid promocodeId, Guid userId)
    {
        return await db.PromocodeUsages
            .AsNoTracking()
            .AnyAsync(u => u.PromocodeId == promocodeId && u.UserId == userId);
    }

    public async Task<PromocodeActivationResult> ApplyPayoutAsync(
        Guid userId,
        string code)
    {
        try
        {
            var result = await db.ExecuteInTransactionAsync<PromocodeActivationResult>(async () =>
            {
                var promocode = await db.Promocodes
                    .ForUpdate()
                    .FirstOrDefaultAsync(p => p.Code == code);

                if (promocode == null)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.NotFound,
                    };

                var balance = await db.Balances
                    .ForUpdate()
                    .FirstOrDefaultAsync(b => b.UserId == userId);

                if (balance == null)
                    throw new Exception("Balance not found (should not happen)");

                if (promocode.BonusType != PromocodeBonusType.Payout)
                    return new PromocodeActivationWrongUsageResult
                    {
                        Outcome = PromocodeActivationOutcome.WrongUsage,
                        BonusType = promocode.BonusType,
                    };

                if (promocode.Bonus.Type != PromocodeBonusType.Payout)
                    return new PromocodeActivationWrongUsageResult
                    {
                        Outcome = PromocodeActivationOutcome.WrongUsage,
                        BonusType = promocode.Bonus.Type,
                    };

                if (promocode.UserId.HasValue && promocode.UserId != userId)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.NotFound,
                    };

                if (await IsUsedAsync(promocode.Id, userId))
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.AlreadyUsed,
                    };

                if (!promocode.IsActive)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.Inactive,
                    };

                if (promocode.ExpiresAt.HasValue && DateTime.UtcNow >= promocode.ExpiresAt)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.Expired,
                    };

                if (promocode.Usages >= promocode.MaxUsages)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.UsageExceeded,
                    };

                var risk = await fraudService.ActualizeRiskAsync(userId);

                if (risk is FraudRisk.High or FraudRisk.Medium)
                    return new PromocodeActivationFailedResult
                    {
                        Outcome = PromocodeActivationOutcome.Blocked,
                    };

                var payout = ((PromocodeBonusPayout)promocode.Bonus).Payout;
                var wageringRequired = (long)Math.Ceiling(payout * (promocode.WageringMultiplier / 100.0));

                promocode.Usages++;
                db.Promocodes.Update(promocode);

                await db.PromocodeUsages.AddAsync(new PromocodeUsageEntity
                {
                    Status = PromocodeUsageStatus.Applied,
                    UserId = userId,
                    PromocodeId = promocode.Id,
                    CreatedAt = DateTime.UtcNow,
                });

                var transaction = new TransactionEntity
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = TransactionType.Bonus,
                    Amount = payout,
                    CreatedAt = DateTime.UtcNow,
                };

                await db.Transactions.AddAsync(transaction);

                balance.Available += transaction.Amount;
                balance.WageringRequired = Math.Max(0, balance.WageringRequired + wageringRequired);
                db.Balances.Update(balance);

                await db.SaveChangesAsync();

                return new PromocodeActivationPayoutResult
                {
                    Outcome = PromocodeActivationOutcome.AppliedPayout,
                    Payout = payout,
                    UpdatedBalance = balance,
                    UpdatedPromocode = promocode,
                    WageringRequired = wageringRequired,
                };
            });

            await cache.ExecuteIfConnectedAsync(async () =>
            {
                if (result is PromocodeActivationPayoutResult payoutResult)
                {
                    var balance = balanceMapper.ToData(payoutResult.UpdatedBalance);
                    var promocode = promocodeMapper.ToData(payoutResult.UpdatedPromocode);
                    await cache.UserBalance.SetAsync(userId, balance);
                    await cache.Promocode.SetAsync(code, promocode);
                }
            });

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to apply Payout");
            return new PromocodeActivationFailedResult { Outcome = PromocodeActivationOutcome.Failed };
        }
    }
}