using Common.Db.Extensions;
using Core.Contracts.Compounds;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Contracts.Public;
using Core.Contracts.Updates;
using Core.Db.Context;
using Core.Services.Auth;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.Promotions;
using Core.Services.PubSub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Core.API.Controllers;

public sealed record ConnectResponse
{
    public required string Status { get; init; } = "success";
}

public sealed record GetCampaignsResponse
{
    public required ReferralCampaign[] Campaigns { get; init; }
    public required int TotalCount { get; init; }
}

public sealed record GetLastTransactionsResponse
{
    public required ReferrerTransaction[] Transactions { get; init; }
    public required long TotalAmount { get; init; }
}

public sealed record WithdrawResponse
{
    public required UserUpdate<BalancePublic> Balance { get; init; }
    public required UserUpdate<ReferrerBalance> ReferrerBalance { get; init; }
}

[Authorize]
[ApiController]
public class AffiliateController(
    CoreDbContext db,
    CoreCacheRegistry cache,
    CorePubSubRegistry pubSub,
    AffiliateService affiliateService,
    BalanceService balanceService,
    BalanceMapper balanceMapper,
    ReferrerTransactionMapper referrerTransactionMapper) : ControllerBase
{
    [ProducesResponseType<ConnectResponse>(StatusCodes.Status200OK)]
    [HttpPost("/affiliate/connect")]
    public async Task<Results<Ok<ConnectResponse>, ValidationProblem>> Connect()
    {
        var userId = HttpContext.GetUserId();
        var settings = await affiliateService.GetReferrerSettingsAsync(userId);

        if (settings is not null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [""] = ["Вы уже подключены к партнерской программе"],
            });

        await db.ExecuteInTransactionAsync(async () =>
        {
            await db.ReferrerSettings.AddAsync(new ReferrerSettingsEntity
            {
                ReferrerId = userId,
                RevShare = 25,
            });

            await db.ReferrerBalances.AddAsync(new ReferrerBalanceEntity
            {
                ReferrerId = userId,
                Available = 0,
            });

            await affiliateService.CreateReferralCampaignAsync(userId, "Основная кампания");
            await affiliateService.CreateReferrerPayoutAsync(userId);
        });

        return TypedResults.Ok(new ConnectResponse { Status = "success" });
    }

    [ProducesResponseType<ReferrerBalance>(StatusCodes.Status200OK)]
    [HttpGet("/affiliate/balance")]
    public async Task<Results<Ok<ReferrerBalance>, ValidationProblem>> GetBalance()
    {
        var userId = HttpContext.GetUserId();
        var balance = await affiliateService.GetReferrerBalanceAsync(userId);

        if (balance is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [""] = ["Вы не подключены к партнерской программе"],
            });

        return TypedResults.Ok(balance);
    }

    [ProducesResponseType<GetCampaignsResponse>(StatusCodes.Status200OK)]
    [HttpGet("/affiliate/campaigns")]
    public async Task<Results<Ok<GetCampaignsResponse>, ValidationProblem>> GetCampaigns([FromQuery] int offset = 0)
    {
        var userId = HttpContext.GetUserId();
        var settings = await affiliateService.GetReferrerSettingsAsync(userId);

        if (settings is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [""] = ["Вы не подключены к партнерской программе"],
            });

        var campaigns = await db.ReferralCampaigns
            .Where(c => c.ReferrerId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(offset)
            .Take(10)
            .ToArrayAsync();

        var totalCount = await db.ReferralCampaigns
            .CountAsync(c => c.ReferrerId == userId);

        return TypedResults.Ok(new GetCampaignsResponse
        {
            Campaigns = campaigns.Select(c => new ReferralCampaign
            {
                Id = c.Id,
                CreatedAt = c.CreatedAt,
                Name = c.Name,
                Code = c.Code,
                TotalVisits = c.TotalVisits,
                TotalSignups = c.TotalSignups,
                ReferrerId = c.ReferrerId,
            }).ToArray(),
            TotalCount = totalCount,
        });
    }

    [ProducesResponseType<GetLastTransactionsResponse>(StatusCodes.Status200OK)]
    [HttpGet("/affiliate/last-transactions")]
    public async Task<Results<Ok<GetLastTransactionsResponse>, ValidationProblem>> GetLastTransactions()
    {
        var userId = HttpContext.GetUserId();
        var settings = await affiliateService.GetReferrerSettingsAsync(userId);

        if (settings is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [""] = ["Вы не подключены к партнерской программе"],
            });

        if (cache.IsReady())
        {
            var cached = await cache.LastReferrerTransactions.GetAsync(userId);
            if (cached is not null)
                return TypedResults.Ok(new GetLastTransactionsResponse
                {
                    Transactions = cached.Transactions,
                    TotalAmount = cached.TotalAmount,
                });
        }

        var payout = await db.ReferrerPayouts
            .FirstOrDefaultAsync(p => p.ReferrerId == userId);

        if (payout is null)
            throw new InvalidOperationException("Should have ReferrerPayout");

        var transactionEntities = await db.ReferrerTransactions
            .Where(t =>
                t.ReferrerId == userId &&
                !t.IsProcessed)
            .OrderByDescending(t => t.Id)
            .ToArrayAsync();

        var transactions = transactionEntities.Select(referrerTransactionMapper.ToData).ToArray();

        var totalAmount = transactionEntities.Sum(t => t.Amount);

        await cache.ExecuteIfConnectedAsync(async () =>
        {
            await cache.LastReferrerTransactions.SetAsync(userId, new LastReferrerTransactions
            {
                Transactions = transactions,
                TotalAmount = totalAmount,
            });
        });

        return TypedResults.Ok(new GetLastTransactionsResponse
        {
            Transactions = transactions,
            TotalAmount = totalAmount,
        });
    }

    [ProducesResponseType<ReferrerSettings>(StatusCodes.Status200OK)]
    [HttpGet("/affiliate/get-settings")]
    public async Task<Results<Ok<ReferrerSettings>, ValidationProblem>> GetSettings()
    {
        var userId = HttpContext.GetUserId();
        var settings = await affiliateService.GetReferrerSettingsAsync(userId);

        if (settings is null)
            return TypedResults.ValidationProblem(new Dictionary<string, string[]>
            {
                [""] = ["Вы не подключены к партнерской программе"],
            });

        return TypedResults.Ok(settings);
    }

    [ProducesResponseType<bool>(StatusCodes.Status200OK)]
    [HttpGet("/affiliate/is-connected")]
    public async Task<Ok<bool>> IsConnected()
    {
        var userId = HttpContext.GetUserId();
        var settings = await affiliateService.GetReferrerSettingsAsync(userId);
        return TypedResults.Ok(settings is not null);
    }

    [ProducesResponseType<WithdrawResponse>(StatusCodes.Status200OK)]
    [HttpPost("/affiliate/withdraw")]
    public async Task<Results<Ok<WithdrawResponse>, ValidationProblem>> Withdraw()
    {
        var userId = HttpContext.GetUserId();

        var (updatedBalance, updatedReferrerBalance) = await db.ExecuteInTransactionAsync(async () =>
        {
            var referrerBalance = await db.ReferrerBalances
                .FirstOrDefaultAsync(b => b.ReferrerId == userId);

            if (referrerBalance is null)
                throw new BadHttpRequestException("Вы не подключены к партнерской программе");

            if (referrerBalance.Available < 1)
                throw new BadHttpRequestException("У вас нет доступных средств для вывода");

            var balance = await db.Balances
                .FirstOrDefaultAsync(b => b.UserId == userId);

            if (balance is null)
                throw new InvalidOperationException("Balance not found");

            var transaction = await balanceService.CreateTransactionAsync(new TransactionEntity
            {
                UserId = userId,
                Type = TransactionType.Transfer,
                Amount = referrerBalance.Available,
                CreatedAt = DateTime.UtcNow,
            });

            var updatedBalance = await balanceService.UpdateBalanceAsync(balance, transaction);

            await affiliateService.CreateReferrerWithdrawalAsync(userId, referrerBalance.Available);

            var updatedReferrerBalance = await affiliateService.UpdateReferrerBalanceAsync(userId, 0);

            return (updatedBalance, updatedReferrerBalance);
        });

        await cache.ExecuteIfConnectedAsync(async () =>
        {
            await cache.UserBalance.SetAsync(userId, balanceMapper.ToData(updatedBalance));
            await cache.ReferrerBalance.SetAsync(userId, updatedReferrerBalance);
        });

        var time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var balanceUpdate = new UserUpdate<BalancePublic>
        {
            Time = time,
            UserId = userId,
            Mode = UpdateMode.Optimized,
            Data = balanceMapper.ToPublic(updatedBalance),
        };

        await pubSub.BalanceUpdated.PublishAsync(balanceUpdate);

        return TypedResults.Ok(new WithdrawResponse
        {
            Balance = balanceUpdate,
            ReferrerBalance = new UserUpdate<ReferrerBalance>
            {
                Time = time,
                UserId = userId,
                Mode = UpdateMode.Optimized,
                Data = updatedReferrerBalance,
            },
        });
    }
}