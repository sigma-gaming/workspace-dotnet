namespace Core.Contracts.Data;

public sealed record ReferrerSettings
{
    public required Guid ReferrerId { get; init; }
    public required int RevShare { get; init; }
} 