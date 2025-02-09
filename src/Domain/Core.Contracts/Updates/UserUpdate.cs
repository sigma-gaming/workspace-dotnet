namespace Core.Contracts.Updates;

public sealed record UserUpdate<T>
{
    public required Guid UserId { get; init; }
    public long Time { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public required UpdateMode Mode { get; init; }
    public required T Data { get; init; }
}