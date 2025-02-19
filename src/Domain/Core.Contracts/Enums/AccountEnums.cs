using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountProvider
{
    Vk,
    Telegram,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserRole
{
    User,
    Admin,
    Moderator,
    Support,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum FraudRisk
{
    Clear,
    Low,
    Medium,
    High,
    Unknown,
}