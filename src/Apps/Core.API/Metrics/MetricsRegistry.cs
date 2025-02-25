using Core.Contracts.Data;
using Prometheus;

namespace Core.API.Metrics;

public static class MetricsRegistry
{
    public static void UpdateBets(GameRecord gameRecord)
    {
        Games.BetsAmount.WithLabels(gameRecord.Game.ToString()).Inc(gameRecord.Bet);
    }

    public static class Games
    {
        public static Counter BetsAmount =>
            Prometheus.Metrics.CreateCounter("games_bets_amount", "Amount of bets", "game");

        public static Histogram DiceSides =>
            Prometheus.Metrics.CreateHistogram("games_dice_sides", "Dice sides", []);

        public static Counter PincodeGames =>
            Prometheus.Metrics.CreateCounter("games_pincode_mode", "Pincode mode", "mode");
    }
}