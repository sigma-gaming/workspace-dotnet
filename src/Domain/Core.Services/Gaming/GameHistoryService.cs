using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;
using Core.Db.Context;

namespace Core.Services.Gaming;

public class GameHistoryService(
    CoreDbContext db,
    CoreCacheRegistry cache)
{
    public async Task<GameRecord[]> GetLastWinHistoryAsync()
    {
        if (cache.IsReady() && await cache.LastWinHistory.ExistsAsync())
        {
            var cached = await cache.LastWinHistory.GetAllAsync();
            return cached.ToArray();
        }

        var records = await db.GameRecords
            .Where(r => r.Outcome == GameOutcome.Win)
            .OrderByDescending(r => r.Id)
            .Take(10)
            .Select(r => new GameRecord
            {
                Id = r.Id,
                Game = r.Game,
                Outcome = r.Outcome,
                CreatedAt = r.CreatedAt,
                Snapshot = r.Snapshot,
                Bet = r.Bet,
                Multiplier = r.Multiplier,
                Payout = r.Payout,
                PreviewUserName = r.PreviewUserName,
                UserId = r.UserId,
                TransactionId = r.TransactionId
            })
            .ToArrayAsync();

        if (cache.IsReady())
            await cache.LastWinHistory.SetAllAsync(records.ToList());

        return records;
    }

    public async Task<GameRecord[]> GetBigWinHistoryAsync()
    {
        if (cache.IsReady() && await cache.BigWinHistory.ExistsAsync())
        {
            var cached = await cache.BigWinHistory.GetAllAsync();
            return cached.ToArray();
        }

        var records = await (
                from r in db.GameRecords
                where r.Multiplier >= 150 && r.Bet + r.Payout >= 3000
                orderby r.Id descending
                select new GameRecord
                {
                    Id = r.Id,
                    Game = r.Game,
                    Outcome = r.Outcome,
                    CreatedAt = r.CreatedAt,
                    Snapshot = r.Snapshot,
                    Bet = r.Bet,
                    Multiplier = r.Multiplier,
                    Payout = r.Payout,
                    PreviewUserName = r.PreviewUserName,
                    UserId = r.UserId,
                    TransactionId = r.TransactionId
                })
            .Take(10)
            .ToArrayAsync();

        if (cache.IsReady())
            await cache.BigWinHistory.SetAllAsync(records.ToList());

        return records;
    }

    public async Task<GameRecord[]> GetUserGameHistoryAsync(Guid userId)
    {
        if (cache.IsReady() && await cache.UserGameHistory.ExistsAsync(userId))
        {
            var cached = await cache.UserGameHistory.GetAllAsync(userId);
            return cached.ToArray();
        }

        var records = await db.GameRecords
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.Id)
            .Take(10)
            .Select(r => new GameRecord
            {
                Id = r.Id,
                Game = r.Game,
                Outcome = r.Outcome,
                CreatedAt = r.CreatedAt,
                Snapshot = r.Snapshot,
                Bet = r.Bet,
                Multiplier = r.Multiplier,
                Payout = r.Payout,
                PreviewUserName = r.PreviewUserName,
                UserId = r.UserId,
                TransactionId = r.TransactionId
            })
            .ToArrayAsync();

        if (cache.IsReady())
            await cache.UserGameHistory.SetAllAsync(userId, records.ToList());

        return records;
    }

    public async Task AddGameRecordAsync(GameRecord record)
    {
        if (!cache.IsReady())
            return;

        await cache.UserGameHistory.LeftPushAsync(record.UserId, record);

        if (record.Bet + record.Payout >= 3000 && record.Multiplier >= 150)
            await cache.BigWinHistory.LeftPushAsync(record);

        if (record.Outcome == GameOutcome.Win)
            await cache.LastWinHistory.LeftPushAsync(record);
    }
}