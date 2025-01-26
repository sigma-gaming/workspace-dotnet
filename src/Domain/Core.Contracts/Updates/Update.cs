namespace Core.Contracts.Updates;

public sealed record Update<T>
{
    public required long Time { get; init; }
    public required UpdateMode Mode { get; init; }
    public required T Data { get; init; }
}