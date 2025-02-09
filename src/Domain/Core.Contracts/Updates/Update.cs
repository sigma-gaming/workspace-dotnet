namespace Core.Contracts.Updates;

public sealed record Update<T>
{
    public long Time { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public required UpdateMode Mode { get; init; }
    public required T Data { get; init; }
}