namespace Core.Services.Auth;

public record SessionOptions
{
    public string SessionIdKey { get; init; } = "session_id";
    public string SessionExpiresKey { get; init; } = "session_expires_at";
}