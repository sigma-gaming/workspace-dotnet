using System.Text.Json.Serialization;

namespace Core.Contracts.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticationAction
{
    SignIn,
    Connect,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticationIntegration
{
    Vk,
    Telegram,
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AuthenticateOutcome
{
    SignedIn,
    SignedUp,
    Connected,
    NotAuthenticated,
}