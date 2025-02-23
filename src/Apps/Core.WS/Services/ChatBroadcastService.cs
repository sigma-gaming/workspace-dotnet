using Core.Services.PubSub;
using Core.WS.Contracts;
using Core.WS.Extensions;
using Core.WS.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Core.WS.Services;

public class ChatBroadcastService(
    CorePubSubRegistry pubSubRegistry,
    IHubContext<MainHub> mainHub,
    ILogger<UpdatesBroadcastService> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken ct)
    {
        pubSubRegistry.ChatMessages.Subscribe(async message =>
        {
            try
            {
                await mainHub.Clients.All
                    .TypedSendAsync(new EventChatMessageCreated(message), ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send maintenance started event");
            }
        }, ct);

        return Task.CompletedTask;
    }
}