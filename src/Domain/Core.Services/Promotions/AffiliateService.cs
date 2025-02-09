using Common.Db.Extensions;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.Finance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Promotions;

public class AffiliateService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ILogger<AffiliateService> logger,
    BalanceService balanceService,
    ReferralCampaignMapper campaignMapper,
    ReferrerSettingsMapper settingsMapper,
    ReferrerBalanceMapper balanceMapper,
    ReferrerPayoutMapper payoutMapper,
    ReferrerWithdrawalMapper withdrawalMapper)
{
    private const string CodeAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const int DefaultCodeLength = 6;
    private const decimal RtpFee = 0.05m;
    private const decimal PaymentFee = 0.05m;
    private const decimal TotalFee = RtpFee + PaymentFee;

    private static DateTime GenerateNextPayoutDate()
    {
        var current = DateTime.UtcNow.AddDays(1); // Skip today

        while (current.Day != 1 && current.Day != 15) current = current.AddDays(1);

        // Distribute payout time randomly between 2 and 6 hours
        var random = new Random();
        var hour = random.Next(2, 7);

        return new DateTime(current.Year, current.Month, current.Day, hour, 0, 0, DateTimeKind.Utc);
    }

    private static string GenerateCode(int length = DefaultCodeLength)
    {
        var random = new Random();
        return new string(Enumerable.Range(0, length)
            .Select(_ => CodeAlphabet[random.Next(CodeAlphabet.Length)])
            .ToArray());
    }

    public async Task<ReferralCampaign?> TryGetCampaignAsync(string code, CancellationToken ct = default)
    {
        var campaign = await db.ReferralCampaigns
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code == code, ct);

        return campaign != null ? campaignMapper.ToData(campaign) : null;
    }

    public async Task IncrementCampaignVisitsAsync(Guid campaignId)
    {
        var campaignEntity = await db.ReferralCampaigns.FindAsync(campaignId);
        if (campaignEntity != null)
        {
            campaignEntity.TotalVisits++;
            await db.SaveChangesAsync();
        }
    }

    public async Task IncrementCampaignSignupsAsync(Guid campaignId)
    {
        var campaignEntity = await db.ReferralCampaigns.FindAsync(campaignId);
        if (campaignEntity != null)
        {
            campaignEntity.TotalSignups++;
            await db.SaveChangesAsync();
        }
    }

    private async Task<ReferrerSettings?> QueryReferrerSettingsAsync(Guid referrerId, CancellationToken ct = default)
    {
        var settingsEntity = await db.ReferrerSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.ReferrerId == referrerId, ct);

        return settingsEntity != null ? settingsMapper.ToData(settingsEntity) : null;
    }

    public async Task<ReferrerSettings?> GetReferrerSettingsAsync(Guid referrerId)
    {
        if (!cache.IsReady())
            return await QueryReferrerSettingsAsync(referrerId);

        var cached = await cache.ReferrerSettings.GetAsync(referrerId);
        if (cached != null)
            return cached;

        var settings = await QueryReferrerSettingsAsync(referrerId);
        if (settings != null)
            await cache.ReferrerSettings.SetAsync(referrerId, settings);

        return settings;
    }

    private async Task<ReferrerBalance?> QueryReferrerBalanceAsync(Guid referrerId, CancellationToken ct = default)
    {
        var balanceEntity = await db.ReferrerBalances
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.ReferrerId == referrerId, ct);

        return balanceEntity != null ? balanceMapper.ToData(balanceEntity) : null;
    }

    public async Task<ReferrerBalance?> GetReferrerBalanceAsync(Guid referrerId)
    {
        if (!cache.IsReady())
            return await QueryReferrerBalanceAsync(referrerId);

        var cached = await cache.ReferrerBalance.GetAsync(referrerId);
        if (cached != null)
            return cached;

        var balance = await QueryReferrerBalanceAsync(referrerId);
        if (balance != null)
            await cache.ReferrerBalance.SetAsync(referrerId, balance);

        return balance;
    }

    public async Task<ReferralCampaign> CreateReferralCampaignAsync(
        Guid referrerId,
        string name)
    {
        var campaign = new ReferralCampaignEntity
        {
            ReferrerId = referrerId,
            Name = name,
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow,
        };

        var campaignEntry = await db.ReferralCampaigns.AddAsync(campaign);
        await db.SaveChangesAsync();

        return campaignMapper.ToData(campaignEntry.Entity);
    }

    public async Task<ReferrerPayout> CreateReferrerPayoutAsync(Guid referrerId)
    {
        var payout = new ReferrerPayoutEntity
        {
            ReferrerId = referrerId,
            NextPayoutAt = GenerateNextPayoutDate(),
        };

        var payoutEntry = await db.ReferrerPayouts.AddAsync(payout);
        await db.SaveChangesAsync();

        return payoutMapper.ToData(payoutEntry.Entity);
    }

    public async Task<ReferrerWithdrawal> CreateReferrerWithdrawalAsync(Guid referrerId, long amount)
    {
        var withdrawal = new ReferrerWithdrawalEntity
        {
            ReferrerId = referrerId,
            Amount = amount,
        };

        var withdrawalEntry = await db.ReferrerWithdrawals.AddAsync(withdrawal);
        await db.SaveChangesAsync();

        return withdrawalMapper.ToData(withdrawalEntry.Entity);
    }

    public async Task<ReferrerBalance> UpdateReferrerBalanceAsync(Guid referrerId, long available)
    {
        await db.ReferrerBalances.Where(b => b.ReferrerId == referrerId).ExecuteUpdateAsync(setters =>
            setters.SetProperty(b => b.Available, available));

        var updatedEntity = await db.ReferrerBalances.FindAsync(referrerId);

        if (updatedEntity == null)
            throw new Exception("ReferrerBalance not found");

        var result = balanceMapper.ToData(updatedEntity);

        await cache.ExecuteIfConnectedAsync(async () => await cache.ReferrerBalance.SetAsync(referrerId, result));

        return result;
    }

    public async Task ProcessReferralTransactionAsync(
        Guid referralId,
        Guid? referrerId,
        Guid? referralCampaignId,
        ReferralAction referralAction,
        Profile referralProfile,
        long amount)
    {
        if (!referrerId.HasValue)
            return;

        var settings = await GetReferrerSettingsAsync(referrerId.Value);
        if (settings == null)
            return;

        var finalAmount = (amount - amount * TotalFee) * (settings.RevShare / 100m);

        var transactionEntity = new ReferrerTransactionEntity
        {
            ReferrerId = referrerId.Value,
            ReferralAction = referralAction,
            ReferralCampaignId = referralCampaignId,
            ReferralId = referralId,
            ReferralName = referralProfile.Name,
            ReferralImage = referralProfile.Image,
            ReferralUsername = referralProfile.Username,
            Amount = (long)finalAmount,
        };

        await db.ReferrerTransactions.AddAsync(transactionEntity);
        await db.SaveChangesAsync();
    }

    public async Task ProcessReferrerPayoutsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var nextPayoutAt = GenerateNextPayoutDate();

        // Get last transaction ID to prevent false-positive isProcessed updates
        var lastTransaction = await db.ReferrerTransactions
            .OrderByDescending(t => t.Id)
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        if (lastTransaction == null)
        {
            logger.LogInformation("No transactions to process");
            return;
        }

        var pendingPayouts = await (from t in db.ReferrerTransactions
            join p in db.ReferrerPayouts on t.ReferrerId equals p.ReferrerId
            where p.NextPayoutAt <= now
            group t by t.ReferrerId
            into g
            select new
            {
                ReferrerId = g.Key,
                TotalAmount = g.Sum(x => x.Amount),
            }).ToListAsync(ct);

        if (pendingPayouts.Count == 0)
        {
            logger.LogInformation("No pending payouts");
            return;
        }

        foreach (var payout in pendingPayouts)
        {
            if (payout.TotalAmount == 0)
                continue;

            await db.ExecuteInTransactionAsync(async () =>
            {
                var balanceEntity = await balanceService.GetBalanceForUpdateAsync(payout.ReferrerId, ct);

                balanceEntity.Available += payout.TotalAmount;

                // Mark transactions as processed
                var transactions = await db.ReferrerTransactions
                    .Where(t => t.Id <= lastTransaction.Id &&
                                t.ReferrerId == payout.ReferrerId &&
                                !t.IsProcessed)
                    .ToListAsync(ct);

                foreach (var t in transactions) t.IsProcessed = true;

                // Update payout dates
                var payoutEntity = await db.ReferrerPayouts
                    .FirstOrDefaultAsync(p => p.ReferrerId == payout.ReferrerId);

                if (payoutEntity != null)
                {
                    payoutEntity.NextPayoutAt = nextPayoutAt;
                    payoutEntity.LastPayoutAt = now;
                }

                await db.SaveChangesAsync();
            });

            await cache.ExecuteIfConnectedAsync(async () =>
            {
                await cache.ReferrerBalance.DeleteAsync(payout.ReferrerId);
                await cache.LastReferrerTransactions.DeleteAsync(payout.ReferrerId);
            });
        }
    }
}