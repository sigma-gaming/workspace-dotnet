using Core.Contracts.Enums;
using Core.Contracts.Jsonb;
using Core.Db.Context;
using Core.Services.Finance;

namespace Core.Services.Gaming.Runners;

public sealed record DiceGamePayload : BaseGamePayload
{
    public required int[] Sides { get; init; }
}

public class DiceGame(
    CoreDbContext db,
    BalanceService balanceService,
    GameService gameService) : BaseGame<DiceGamePayload>(db, balanceService, gameService)
{
    private readonly Random random = Random.Shared;

    protected override GameRunnerResult RunGameAsync(DiceGamePayload payload)
    {
        var sides = new HashSet<int>(payload.Sides);

        var multiplier = DiceCalculator.CalculateMultiplier(sides);
        var winAmount = DiceCalculator.CalculateWinAmount(payload.Bet, sides, multiplier);

        var side = random.Next(1, 7);
        var outcome = sides.Contains(side) ? GameOutcome.Win : GameOutcome.Loss;
        var payout = outcome == GameOutcome.Win ? winAmount : -payload.Bet;

        return new GameRunnerResult
        {
            Game = Game.Dice,
            Payout = payout,
            Outcome = outcome,
            Snapshot = new GameSnapshotDice
            {
                InputSides = sides.ToArray(),
                OutputSide = side,
            },
        };
    }
}