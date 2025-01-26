namespace Core.Contracts.Public;

public sealed record BalancePublic
{
    public required Guid UserId { get; init; }
    public required long Available { get; init; }
}