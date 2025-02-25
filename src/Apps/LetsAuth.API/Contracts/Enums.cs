using System.Text.Json.Serialization;

namespace LetsAuth.API.Contracts;

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