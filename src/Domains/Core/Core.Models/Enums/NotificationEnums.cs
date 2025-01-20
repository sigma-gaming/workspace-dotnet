using System.Text.Json.Serialization;

namespace Core.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum NotificationKind
{
    Success,
    Info,
    Warning,
    Failure
}