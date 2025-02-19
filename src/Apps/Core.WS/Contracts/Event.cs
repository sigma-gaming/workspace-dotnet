using System.ComponentModel.DataAnnotations;
using Core.Contracts.Data;
using Core.Contracts.Public;
using Core.Contracts.Updates;

namespace Core.WS.Contracts;

public sealed record EmptyPayload;

public abstract record Event(object Payload)
{
    [Required] public abstract EventName Name { get; }
}

public sealed record EventNotificationCreated(Notification Payload) : Event(Payload)
{
    public override EventName Name => EventName.NotificationCreated;
    [Required] public new Notification Payload => (Notification)base.Payload;
}

public sealed record EventBalanceUpdated(UserUpdate<BalancePublic> Payload) : Event(Payload)
{
    public override EventName Name => EventName.BalanceUpdated;
    [Required] public new UserUpdate<BalancePublic> Payload => (UserUpdate<BalancePublic>)base.Payload;
}

public sealed record EventGlobalTaskStatusUpdated(UserUpdate<GlobalTaskStatus> Payload) : Event(Payload)
{
    public override EventName Name => EventName.GlobalTaskStatusUpdated;
    [Required] public new UserUpdate<GlobalTaskStatus> Payload => (UserUpdate<GlobalTaskStatus>)base.Payload;
}

public sealed record EventMaintenanceStarted(EmptyPayload Payload) : Event(Payload)
{
    public override EventName Name => EventName.MaintenanceStarted;
    [Required] public new EmptyPayload Payload => (EmptyPayload)base.Payload;
}

public sealed record EventLastWinsUpdated(GameRecord[] Payload) : Event(Payload)
{
    public override EventName Name => EventName.LastWinsUpdated;
    [Required] public new GameRecord[] Payload => (GameRecord[])base.Payload;
}

public sealed record EventBigWinsUpdated(GameRecord[] Payload) : Event(Payload)
{
    public override EventName Name => EventName.BigWinsUpdated;
    [Required] public new GameRecord[] Payload => (GameRecord[])base.Payload;
}

public sealed record EventChatMessageCreated(ChatMessage Payload) : Event(Payload)
{
    public override EventName Name => EventName.ChatMessageCreated;
    [Required] public new ChatMessage Payload => (ChatMessage)base.Payload;
}