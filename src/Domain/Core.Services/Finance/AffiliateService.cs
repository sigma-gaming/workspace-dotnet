using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Core.Services.Finance;

public class AffiliateService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    ILogger<AffiliateService> logger,
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

        while (current.Day != 1 && current.Day != 15)
        {
            current = current.AddDays(1);
        }

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

    public async Task<ReferralCampaign?> GetCampaignAsync(string code)
    {
        var campaign = await db.ReferralCampaigns
            .FirstOrDefaultAsync(c => c.Code == code);

        return campaign != null ? campaignMapper.ToData(campaign) : null;
    }

    public async Task IncrementCampaignVisitsAsync(Guid campaignId)
    {
        var campaign = await db.ReferralCampaigns.FindAsync(campaignId);
        if (campaign != null)
        {
            campaign.TotalVisits++;
            await db.SaveChangesAsync();
        }
    }

    public async Task IncrementCampaignSignupsAsync(Guid campaignId)
    {
        var campaign = await db.ReferralCampaigns.FindAsync(campaignId);
        if (campaign != null)
        {
            campaign.TotalSignups++;
            await db.SaveChangesAsync();
        }
    }

    private async Task<ReferrerSettings?> QueryReferrerSettingsAsync(Guid referrerId)
    {
        var settings = await db.ReferrerSettings
            .FirstOrDefaultAsync(s => s.ReferrerId == referrerId);

        return settings != null ? settingsMapper.ToData(settings) : null;
    }

    public async Task<ReferrerSettings?> GetReferrerSettingsAsync(Guid referrerId)
    {
        if (!cache.Ready())
            return await QueryReferrerSettingsAsync(referrerId);

        var cached = await cache.ReferrerSettings.GetAsync(referrerId);
        if (cached != null)
            return cached;

        var settings = await QueryReferrerSettingsAsync(referrerId);
        if (settings != null)
            await cache.ReferrerSettings.SetAsync(referrerId, settings);

        return settings;
    }

    private async Task<ReferrerBalance?> QueryReferrerBalanceAsync(Guid referrerId)
    {
        var balance = await db.ReferrerBalances
            .FirstOrDefaultAsync(b => b.ReferrerId == referrerId);

        return balance != null ? balanceMapper.ToData(balance) : null;
    }

    public async Task<ReferrerBalance?> GetReferrerBalanceAsync(Guid referrerId)
    {
        if (!cache.Ready())
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
        string name,
        string? description = null)
    {
        var campaign = new ReferralCampaignEntity
        {
            ReferrerId = referrerId,
            Name = name,
            Code = GenerateCode(),
            CreatedAt = DateTime.UtcNow
        };

        var entry = await db.ReferralCampaigns.AddAsync(campaign);
        await db.SaveChangesAsync();

        return campaignMapper.ToData(entry.Entity);
    }

    public async Task<ReferrerPayout> CreateReferrerPayoutAsync(Guid referrerId)
    {
        var payout = new ReferrerPayoutEntity
        {
            ReferrerId = referrerId,
            NextPayoutAt = GenerateNextPayoutDate()
        };

        var entry = await db.ReferrerPayouts.AddAsync(payout);
        await db.SaveChangesAsync();

        return payoutMapper.ToData(entry.Entity);
    }

    public async Task<ReferrerWithdrawal> CreateReferrerWithdrawalAsync(Guid referrerId, long amount)
    {
        var withdrawal = new ReferrerWithdrawalEntity
        {
            ReferrerId = referrerId,
            Amount = amount
        };

        var entry = await db.ReferrerWithdrawals.AddAsync(withdrawal);
        await db.SaveChangesAsync();

        return withdrawalMapper.ToData(entry.Entity);
    }

    public async Task<ReferrerBalance> UpdateReferrerBalanceAsync(Guid referrerId, long available)
    {
        var balance = await db.ReferrerBalances
                          .FirstOrDefaultAsync(b => b.ReferrerId == referrerId)
                      ?? throw new Exception("ReferrerBalance not found");

        balance.Available = available;
        await db.SaveChangesAsync();

        var result = balanceMapper.ToData(balance);

        if (cache.Ready())
            await cache.ReferrerBalance.DeleteAsync(referrerId);

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

        var finalAmount = (amount - amount * (decimal)TotalFee) * (settings.RevShare / 100m);

        var transaction = new ReferrerTransactionEntity
        {
            ReferrerId = referrerId.Value,
            ReferralAction = referralAction,
            ReferralCampaignId = referralCampaignId,
            ReferralId = referralId,
            ReferralName = referralProfile.Name,
            ReferralImage = referralProfile.Image,
            ReferralUsername = referralProfile.Username,
            Amount = (long)finalAmount
        };

        await db.ReferrerTransactions.AddAsync(transaction);
        await db.SaveChangesAsync();
    }

    public async Task ProcessReferrerPayoutsAsync()
    {
        var now = DateTime.UtcNow;
        var nextPayoutAt = GenerateNextPayoutDate();

        // Get last transaction ID to prevent false-positive isProcessed updates
        var lastTransaction = await db.ReferrerTransactions
            .OrderByDescending(t => t.Id)
            .FirstOrDefaultAsync();

        if (lastTransaction == null)
        {
            logger.LogInformation("No transactions to process");
            return;
        }

        var pendingPayouts = await db.ReferrerTransactions
            .Where(t => t.Id <= lastTransaction.Id && !t.IsProcessed)
            .Join(db.ReferrerPayouts,
                t => t.ReferrerId,
                p => p.ReferrerId,
                (t, p) => new { Transaction = t, Payout = p })
            .Where(x => x.Payout.NextPayoutAt <= now)
            .GroupBy(x => x.Transaction.ReferrerId)
            .Select(g => new
            {
                ReferrerId = g.Key,
                TotalAmount = g.Sum(x => x.Transaction.Amount)
            })
            .ToListAsync();

        if (pendingPayouts.Count == 0)
        {
            logger.LogInformation("No pending payouts");
            return;
        }

        foreach (var payout in pendingPayouts)
        {
            if (payout.TotalAmount == 0)
                continue;

            await db.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
            {
                await using var transaction = await db.Database.BeginTransactionAsync();

                try
                {
                    var balance = await db.ReferrerBalances
                        .TagWith("ForUpdate")
                        .FirstOrDefaultAsync(b => b.ReferrerId == payout.ReferrerId);

                    if (balance != null)
                    {
                        balance.Available += payout.TotalAmount;

                        // Mark transactions as processed
                        var transactions = await db.ReferrerTransactions
                            .Where(t => t.Id <= lastTransaction.Id &&
                                        t.ReferrerId == payout.ReferrerId &&
                                        !t.IsProcessed)
                            .ToListAsync();

                        foreach (var t in transactions)
                        {
                            t.IsProcessed = true;
                        }

                        // Update payout dates
                        var payoutEntity = await db.ReferrerPayouts
                            .FirstOrDefaultAsync(p => p.ReferrerId == payout.ReferrerId);

                        if (payoutEntity != null)
                        {
                            payoutEntity.NextPayoutAt = nextPayoutAt;
                            payoutEntity.LastPayoutAt = now;
                        }

                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();

                        if (cache.Ready())
                        {
                            await cache.ReferrerBalance.DeleteAsync(payout.ReferrerId);
                            await cache.LastReferrerTransactions.DeleteAsync(payout.ReferrerId);
                        }
                    }
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }
    }
}