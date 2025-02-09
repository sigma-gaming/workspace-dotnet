using Common.PubSub.Abstractions;
using Core.Contracts.Data;
using Core.Contracts.Public;
using Core.Contracts.Updates;

namespace Core.Services.PubSub;

public class CorePubSubRegistry(IPubSubService pubSubService)
{
    public ITypedPubSub<object> MaintenanceStarted { get; } =
        pubSubService.Create<object>("maintenance-started");

    public ITypedPubSub<Notification> Notifications { get; } =
        pubSubService.Create<Notification>("notifications");

    public ITypedPubSub<ChatMessage> ChatMessages { get; } =
        pubSubService.Create<ChatMessage>("chat-messages");

    public ITypedPubSub<UserUpdate<BalancePublic>> BalanceUpdated { get; } =
        pubSubService.Create<UserUpdate<BalancePublic>>("balance-updated");

    public ITypedPubSub<UserUpdate<GlobalTaskStatus>> GlobalTaskStatusUpdated { get; } =
        pubSubService.Create<UserUpdate<GlobalTaskStatus>>("global-task-status-updated");
}