using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Core.Services.Finance;

namespace Core.Services.Gaming.Runners;

public sealed record PincodeGamePayload : BaseGamePayload
{
    public required PincodeMode Mode { get; init; }
}

public class PincodeGame(
    CoreDbContext db,
    BalanceService balanceService,
    GameService gameService) : BaseGame<PincodeGamePayload>(db, balanceService, gameService)
{
    private readonly Random random = Random.Shared;

    protected override GameRunnerResult RunGameAsync(PincodeGamePayload payload)
    {
        var number = random.Next(10000);
        var multiplier = PincodeCalculator.GetPincodeMultiplier(payload.Mode, number);

        var outcome = multiplier > 0 ? GameOutcome.Win : GameOutcome.Loss;
        var winAmount = payload.Bet * multiplier - payload.Bet;
        var payout = outcome == GameOutcome.Win ? winAmount : -payload.Bet;

        return new GameRunnerResult
        {
            Game = Game.Pincode,
            Payout = payout,
            Outcome = outcome,
            Snapshot = new GameSnapshotPincode
            {
                Mode = payload.Mode,
                OutputNumber = number,
            },
        };
    }
}