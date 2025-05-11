using Core.Contracts.Enums;
using Core.Db.Context;
using Core.Services.Caching;
using Core.Services.Finance;
using Core.Tasks.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;

namespace Core.Tasks.Jobs;

/// <summary>
///     A background job that sends advanced games metrics
/// </summary>
public class SendAdvancedGamesMetricsJob(
    BudgetService budgetService,
    CoreDbContext db,
    CoreCacheRegistry cache,
    IOptions<MetricsOptions> metricsOptions,
    ILogger<SendAdvancedGamesMetricsJob>? logger = null)
{
    private readonly MetricsOptions options = metricsOptions.Value;

    /// <summary>
    ///     Executes the advanced games metrics job
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task ExecuteAsync(CancellationToken ct)
    {
        logger?.LogInformation("Starting advanced games metrics job at: {Time}", DateTimeOffset.Now);

        try
        {
            await using var cacheLock = await cache.LastMetricsTransactionId.AcquireLockAsync(cancellationToken: ct);

            var lastProcessedTransactionId = await cache.LastMetricsTransactionId.GetAsync();
            var budget = await budgetService.GetAvailableAsync(ct);

            // Add some gap to avoid race condition
            var tenSecondsAgo = DateTimeOffset.UtcNow.AddSeconds(-10);

            // Query using proper entities from CoreDbContext
            var stats = await GetTransactionStatsAsync(
                lastProcessedTransactionId,
                tenSecondsAgo,
                [TransactionType.Win, TransactionType.Loss],
                ct);

            logger?.LogDebug("Transaction stats: {Stats}", stats);

            var maxId = stats.Aggregate(Guid.Empty.ToString(),
                (acc, stat) => string.Compare(stat.MaxId, acc, StringComparison.Ordinal) > 0 ? stat.MaxId : acc);

            // If there's no Pushgateway URL configured, just update the last ID and return
            if (string.IsNullOrEmpty(options.PushgatewayUrl))
            {
                if (maxId != Guid.Empty.ToString())
                {
                    await cache.LastMetricsTransactionId.SetAsync(maxId);
                    logger?.LogInformation("Updated lastMetricsTransactionId to {Id}", maxId);
                }

                logger?.LogInformation("Pushgateway URL is not set, skipping metrics push");
                return;
            }

            // Create Prometheus metrics
            var registry = Metrics.DefaultRegistry;

            var budgetGauge =
                Metrics.CreateGauge("games_available_budget", "Available budget in the system");
            var maxAmountGauge = Metrics.CreateGauge("games_max_amount",
                "Maximum amount of win/loss in the period",
                new GaugeConfiguration { LabelNames = ["type", "game"] });
            var totalAmountGauge = Metrics.CreateGauge("games_total_amount",
                "Total amount of win/loss in the period",
                new GaugeConfiguration { LabelNames = ["type", "game"] });
            var gamesCountGauge = Metrics.CreateGauge("games_count",
                "Number of games played in the period",
                new GaugeConfiguration { LabelNames = ["type", "game"] });

            logger?.LogInformation("Budget: {Budget}", budget);
            budgetGauge.Set(Convert.ToDouble(budget));

            // Initialize all metrics with zero for all game types
            foreach (var type in Enum.GetValues<TransactionType>()
                         .Where(t => t is TransactionType.Win or TransactionType.Loss))
            foreach (var game in Enum.GetValues<Game>())
            {
                maxAmountGauge.WithLabels(type.ToString(), game.ToString()).Set(0);
                totalAmountGauge.WithLabels(type.ToString(), game.ToString()).Set(0);
                gamesCountGauge.WithLabels(type.ToString(), game.ToString()).Set(0);
            }

            // Update metrics with actual values
            foreach (var stat in stats)
            {
                if (stat.Game is null) continue;

                logger?.LogInformation("Setting metrics for type {Type} and game {Game}", stat.Type, stat.Game);
                logger?.LogInformation("Max amount: {MaxAmount}", stat.MaxAmount);
                logger?.LogInformation("Total amount: {TotalAmount}", stat.TotalAmount);
                logger?.LogInformation("Count: {Count}", stat.Count);

                maxAmountGauge.WithLabels(stat.Type.ToString(), stat.Game.Value.ToString())
                    .Set(Convert.ToDouble(stat.Type == TransactionType.Win ? stat.MaxAmount : -stat.MinAmount));

                totalAmountGauge.WithLabels(stat.Type.ToString(), stat.Game.Value.ToString())
                    .Set(Convert.ToDouble(Math.Abs(stat.TotalAmount)));

                gamesCountGauge.WithLabels(stat.Type.ToString(), stat.Game.Value.ToString())
                    .Set(stat.Count);
            }

            logger?.LogInformation("Pushing metrics to pushgateway at {Url}", options.PushgatewayUrl);

            // Push metrics to Prometheus Pushgateway
            await PushMetricsAsync(options.PushgatewayUrl, "send-advanced-games-metrics", registry);

            logger?.LogInformation("Metrics pushed to pushgateway");

            // Update the last processed ID
            if (maxId != Guid.Empty.ToString())
            {
                await cache.LastMetricsTransactionId.SetAsync(maxId);
                logger?.LogInformation("Updated lastMetricsTransactionId to {Id}", maxId);
            }

            logger?.LogInformation("Advanced games metrics job completed successfully");
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred during advanced games metrics job");
            throw; // Rethrow to let Hangfire know the job failed
        }
    }

    private async Task<IList<TransactionStat>> GetTransactionStatsAsync(
        string? lastProcessedTransactionId,
        DateTimeOffset maxDate,
        TransactionType[] types,
        CancellationToken cancellationToken)
    {
        // Using the TransactionEntity from CoreDbContext
        var query = db.Transactions.AsQueryable();

        // Filter by transaction ID if provided
        if (!string.IsNullOrEmpty(lastProcessedTransactionId) &&
            Guid.TryParse(lastProcessedTransactionId, out var lastId))
            query = query.Where(t => t.Id > lastId);

        // Apply additional filters
        query = query
            .Where(t => t.CreatedAt <= maxDate)
            .Where(t => types.Contains(t.Type));

        // Group and aggregate data
        var results = await query
            .GroupBy(t => new { t.Type, t.Game })
            .Select(g => new TransactionStat
            {
                Type = g.Key.Type,
                Game = g.Key.Game,
                MaxAmount = g.Max(t => t.Amount),
                MinAmount = g.Min(t => t.Amount),
                TotalAmount = g.Sum(t => t.Amount),
                Count = g.Count(),
                MaxId = g.Max(t => t.Id).ToString(),
            })
            .ToListAsync(cancellationToken);

        return results;
    }

    private async Task PushMetricsAsync(string pushgatewayUrl, string jobName, CollectorRegistry registry)
    {
        try
        {
            var pusher = new MetricPusher(pushgatewayUrl, jobName, registry: registry);

            pusher.Start();
            await pusher.StopAsync();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error pushing metrics to Pushgateway at {Url}", pushgatewayUrl);
            throw;
        }
    }
}

/// <summary>
///     Class to hold transaction statistics
/// </summary>
public class TransactionStat
{
    public TransactionType Type { get; set; }
    public Game? Game { get; set; }
    public decimal MaxAmount { get; set; }
    public decimal MinAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public int Count { get; set; }
    public string MaxId { get; set; } = string.Empty;
}