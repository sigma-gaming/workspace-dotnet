using Core.Services.Caching;
using Core.WS.Contracts;
using Core.WS.Extensions;
using Core.WS.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Core.WS.Services;

public class GameHistoryBroadcastService(
    CoreCacheRegistry cache,
    IHubContext<MainHub> mainHub,
    ILogger<GameHistoryBroadcastService> logger) : BackgroundService
{
    private const int INITIAL_DELAY = 3000;
    private const int DEFAULT_DELAY = 900;
    private const int MAX_DELAY = 4000;
    private Guid lastBigWinSent = Guid.Empty;
    private Guid lastWinSent = Guid.Empty;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(INITIAL_DELAY, stoppingToken);

        await Task.WhenAll(
            BroadcastLastWinsLoop(stoppingToken),
            BroadcastBigWinsLoop(stoppingToken)
        );
    }

    private async Task BroadcastLastWinsLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
            try
            {
                var lastWins = await cache.LastWinHistory.GetAllAsync();

                var newWins = lastWins.Where(record => record.Id.CompareTo(lastWinSent) > 0).ToList();

                if (newWins.Count == 0)
                {
                    await Task.Delay(DEFAULT_DELAY, ct);
                    continue;
                }

                lastWinSent = newWins[0].Id;
                await mainHub.Clients.All
                    .TypedSendAsync(new EventLastWinsUpdated(newWins.ToArray()), ct);

                var delay = Math.Min(MAX_DELAY, 1000 * newWins.Count - 100);
                await Task.Delay(delay, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to send last wins");
                await Task.Delay(DEFAULT_DELAY, ct);
            }
    }

    private async Task BroadcastBigWinsLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
            try
            {
                var bigWins = await cache.BigWinHistory.GetAllAsync();

                var newWins = bigWins.Where(record => record.Id.CompareTo(lastBigWinSent) > 0).ToList();

                if (newWins.Count == 0)
                {
                    await Task.Delay(DEFAULT_DELAY, ct);
                    continue;
                }

                lastBigWinSent = newWins[0].Id;
                await mainHub.Clients.All.TypedSendAsync(new EventBigWinsUpdated(newWins.ToArray()), ct);

                var delay = Math.Min(MAX_DELAY, 1000 * newWins.Count - 100);
                await Task.Delay(delay, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Failed to send big wins");
                await Task.Delay(DEFAULT_DELAY, ct);
            }
    }
}