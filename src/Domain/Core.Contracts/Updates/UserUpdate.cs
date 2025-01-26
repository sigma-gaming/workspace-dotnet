namespace Core.Contracts.Updates;

public sealed record UserUpdate<T>
{
    public required Guid UserId { get; init; }
    public required long Time { get; init; }
    public required UpdateMode Mode { get; init; }
    public required T Data { get; init; }
}