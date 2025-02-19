using System.Text.Json.Serialization;

namespace Core.WS.Contracts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum EventName
{
    MaintenanceStarted,
    BalanceUpdated,
    GlobalTaskStatusUpdated,
    NotificationCreated,
    LastWinsUpdated,
    BigWinsUpdated,
    ChatMessageCreated,
}

public static class EventNameExtensions
{
    public static string Name(this EventName eventName)
    {
        return eventName.ToString();
    }
}