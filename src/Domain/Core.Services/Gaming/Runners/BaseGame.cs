using Common.Db.Extensions;
using Core.Db.Context;
using Core.Services.Finance;

namespace Core.Services.Gaming.Runners;

public class InsufficientBalanceException : Exception
{
}

public abstract class BaseGame<TGamePayload>(
    CoreDbContext db,
    BalanceService balanceService,
    GameService gameService) where TGamePayload : BaseGamePayload
{
    public async Task<GamePlayResult> PlayAsync(Guid userId, TGamePayload payload)
    {
        var result = await db.ExecuteInTransactionAsync(async () =>
        {
            var balance = await balanceService.GetBalanceForUpdateAsync(userId);

            if (balance.Available < payload.Bet) throw new InsufficientBalanceException();

            var result = await gameService.RunGameAsync(() => RunGameAsync(payload));

            var (gameRecordEntity, updatedBalance) = await gameService.SaveGameAsync(new SaveGamePayload
            {
                UserId = userId,
                Game = result.Game,
                Outcome = result.Outcome,
                Snapshot = result.Snapshot,
                Bet = payload.Bet,
                Payout = result.Payout,
                Balance = balance,
            });

            return new GamePlayResult
            {
                GameRecord = gameRecordEntity,
                UpdatedBalance = updatedBalance,
            };
        });

        return result;
    }

    protected abstract GameRunnerResult RunGameAsync(TGamePayload payload);
}