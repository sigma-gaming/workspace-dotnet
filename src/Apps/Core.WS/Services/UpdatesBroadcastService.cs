using Core.Services.PubSub;
using Core.WS.Contracts;
using Core.WS.Extensions;
using Core.WS.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Core.WS.Services;

public class UpdatesBroadcastService(
    CorePubSubRegistry pubSubRegistry,
    IHubContext<MainHub> mainHub,
    ILogger<UpdatesBroadcastService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        pubSubRegistry.BalanceUpdated.Subscribe(async update =>
        {
            try
            {
                await mainHub.Clients
                    .User(update.UserId.ToString())
                    .TypedSendAsync(new EventBalanceUpdated(update), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send balance update to user {UserId}", update.UserId);
            }
        }, ct);

        pubSubRegistry.GlobalTaskStatusUpdated.Subscribe(async update =>
        {
            try
            {
                await mainHub.Clients
                    .User(update.UserId.ToString())
                    .TypedSendAsync(new EventGlobalTaskStatusUpdated(update), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send task status update to user {UserId}", update.UserId);
            }
        }, ct);

        return Task.CompletedTask;
    }
}