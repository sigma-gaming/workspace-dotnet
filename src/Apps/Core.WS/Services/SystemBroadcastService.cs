using Core.Services.PubSub;
using Core.WS.Contracts;
using Core.WS.Extensions;
using Core.WS.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Core.WS.Services;

public class SystemBroadcastService(
    CorePubSubRegistry pubSubRegistry,
    IHubContext<MainHub> mainHub,
    ILogger<UpdatesBroadcastService> logger) : BackgroundService
{
    private readonly List<IDisposable> subscriptions = [];

    protected override Task ExecuteAsync(CancellationToken ct)
    {
        subscriptions.Add(pubSubRegistry.MaintenanceStarted.Subscribe(async _ =>
        {
            try
            {
                await mainHub.Clients.All
                    .TypedSendAsync(new EventMaintenanceStarted(new EmptyPayload()), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send maintenance started event");
            }
        }));

        subscriptions.Add(pubSubRegistry.Notifications.Subscribe(async notification =>
        {
            try
            {
                if (notification.UserId is { } userId)
                    await mainHub.Clients
                        .User(userId.ToString())
                        .TypedSendAsync(new EventNotificationCreated(notification), ct);
                else
                    await mainHub.Clients.All
                        .TypedSendAsync(new EventNotificationCreated(notification), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send notification to user {UserId}", notification.UserId);
            }
        }));

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken ct)
    {
        foreach (var subscription in subscriptions) subscription.Dispose();

        subscriptions.Clear();

        return base.StopAsync(ct);
    }
}