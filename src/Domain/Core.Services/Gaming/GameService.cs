using System.Text.Json;
using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Common.Db.Extensions;
using Core.Contracts.Mappers;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Services.User;
using Microsoft.EntityFrameworkCore;

namespace Core.Services.Gaming;

public sealed record SaveGamePayload
{
    public required Guid UserId { get; init; }
    public required Game Game { get; init; }
    public required GameOutcome Outcome { get; init; }
    public required GameSnapshot Snapshot { get; init; }
    public required long Bet { get; init; }
    public required long Payout { get; init; }
    public required BalanceEntity Balance { get; init; }
}

public sealed record GameRunnerResult<TGame> where TGame : struct
{
    public required TGame Game { get; init; }
    public required long Payout { get; init; }
    public required GameOutcome Outcome { get; init; }
    public required string Snapshot { get; init; }
}

public class GameService(
    CoreDbContext db,
    CoreCacheRegistry cache,
    BudgetService budgetService,
    BalanceService balanceService,
    ProfileService profileService,
    GameHistoryService gameHistoryService,
    GameRecordMapper gameRecordMapper,
    BalanceMapper balanceMapper)
{
    private static readonly Dictionary<GameOutcome, TransactionType> OutcomeToTypeMap = new()
    {
        [GameOutcome.Win] = TransactionType.Win,
        [GameOutcome.Loss] = TransactionType.Loss
    };

    public async Task<GameRecordEntity?> GetGameRecordAsync(Guid gameRecordId, CancellationToken ct = default)
    {
        return await db.GameRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == gameRecordId, ct);
    }

    public decimal CalculateWageringMultiplier(GameSnapshot snapshot)
    {
        switch (snapshot)
        {
            case GameSnapshotDice dice:
                return (6.0m / dice.InputSides.Length - 1) / 5;
            case GameSnapshotPincode pincode:
                return pincode.Mode == PincodeMode.Easy ? 0.75m : 1;
            default:
                throw new InvalidOperationException("Unknown game snapshot type");
        }
    }

    public long CalculateWageringChange(long bet, GameSnapshot snapshot)
    {
        var multiplier = CalculateWageringMultiplier(snapshot);
        return (long)Math.Ceiling(-bet * multiplier);
    }

    public async Task<GameRunnerResult<TGame>> RunGameAsync<TGame>(
        Func<Task<GameRunnerResult<TGame>>> runner,
        long minPayout = 0) where TGame : struct
    {
        var availableBudget = await budgetService.GetAvailableAsync();

        if (minPayout > availableBudget)
            throw new InvalidOperationException("Budget is not enough");

        while (true)
        {
            var result = await runner();

            if (result.Payout <= availableBudget)
                return result;
        }
    }

    public async Task<(GameRecordEntity GameRecord, Balance UpdatedBalance)> SaveGameAsync(SaveGamePayload payload)
    {
        var profile = await profileService.GetUserDetailsAsync(payload.UserId);

        if (profile == null)
            throw new InvalidOperationException("User profile not found");

        var (gameRecord, updatedBalance) = await db.ExecuteInTransactionAsync(async () =>
        {
            var transactionId = Guid.CreateVersion7();
            var gameRecordId = Guid.CreateVersion7();

            var transaction = await balanceService.CreateTransactionAsync(new TransactionEntity
            {
                Id = transactionId,
                UserId = payload.UserId,
                Type = OutcomeToTypeMap[payload.Outcome],
                Game = payload.Game,
                Amount = payload.Payout,
                GameRecordId = gameRecordId,
                CreatedAt = DateTime.UtcNow
            });

            var multiplier = (int)Math.Floor(Math.Max(0, (decimal)payload.Payout / payload.Bet * 100));

            var gameRecordEntity = new GameRecordEntity
            {
                Id = gameRecordId,
                Game = payload.Game,
                Outcome = payload.Outcome,
                Snapshot = payload.Snapshot,
                Multiplier = multiplier,
                Bet = payload.Bet,
                Payout = payload.Payout,
                UserId = payload.UserId,
                PreviewUserName = profile.Profile.Username ?? profile.Profile.Name,
                TransactionId = transactionId,
                CreatedAt = DateTime.UtcNow
            };

            db.GameRecords.Add(gameRecordEntity);
            await db.SaveChangesAsync();

            var wageringChange = CalculateWageringChange(payload.Bet, payload.Snapshot);

            var updatedBalance = await balanceService.UpdateBalanceAsync(
                payload.Balance,
                transaction,
                gameRecordEntity,
                wageringChange);

            return (gameRecord: gameRecordEntity, balanceMapper.ToData(updatedBalance));
        });

        await cache.ExecuteIfConnectedAsync(async () =>
        {
            await cache.UserBalance.SetAsync(payload.UserId, updatedBalance);
            await budgetService.ChangeAvailableAsync(-payload.Payout);
        });

        _ = gameHistoryService.AddGameRecordAsync(gameRecordMapper.ToData(gameRecord));

        return (gameRecord, updatedBalance);
    }
}