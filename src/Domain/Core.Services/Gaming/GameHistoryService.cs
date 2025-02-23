using Core.Contracts.Data;
using Core.Contracts.Enums;
using Core.Db.Context;
using Core.Services.Caching;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Gaming;

public class GameHistoryService(
    CoreDbContext db,
    CoreCacheRegistry cache)
{
    public async Task<GameRecord[]> GetLastWinHistoryAsync(CancellationToken ct = default)
    {
        if (await cache.LastWinHistory.TryExistsAsync())
        {
            var (cached, succeed) = await cache.LastWinHistory.TryGetAllAsync();
            if (succeed) return cached.ToArray();
        }

        var records = await (
                from r in db.GameRecords
                where r.Outcome == GameOutcome.Win
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
                    TransactionId = r.TransactionId,
                })
            .Take(10)
            .AsNoTracking()
            .ToArrayAsync(ct);

        await cache.LastWinHistory.TrySetAllAsync(records.ToList());

        return records;
    }

    public async Task<GameRecord[]> GetBigWinHistoryAsync(CancellationToken ct = default)
    {
        if (await cache.BigWinHistory.TryExistsAsync())
        {
            var (cached, succeed) = await cache.BigWinHistory.TryGetAllAsync();
            if (succeed) return cached.ToArray();
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
                    TransactionId = r.TransactionId,
                })
            .Take(10)
            .AsNoTracking()
            .ToArrayAsync(ct);

        await cache.BigWinHistory.TrySetAllAsync(records.ToList());

        return records;
    }

    public async Task<GameRecord[]> GetUserGameHistoryAsync(Guid userId, CancellationToken ct = default)
    {
        if (await cache.UserGameHistory.TryExistsAsync(userId))
        {
            var (cached, succeed) = await cache.UserGameHistory.TryGetAllAsync(userId);
            if (succeed) return cached.ToArray();
        }

        var records = await (
                from r in db.GameRecords
                where r.UserId == userId
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
                    TransactionId = r.TransactionId,
                })
            .Take(10)
            .AsNoTracking()
            .ToArrayAsync(ct);

        await cache.UserGameHistory.TrySetAllAsync(userId, records.ToList());

        return records;
    }

    public async Task AddGameRecordAsync(GameRecord record)
    {
        await cache.UserGameHistory.TryLeftPushAsync(record.UserId, record);

        if (record.Bet + record.Payout >= 3000 && record.Multiplier >= 150)
            await cache.BigWinHistory.TryLeftPushAsync(record);

        if (record.Outcome == GameOutcome.Win)
            await cache.LastWinHistory.TryLeftPushAsync(record);
    }
}