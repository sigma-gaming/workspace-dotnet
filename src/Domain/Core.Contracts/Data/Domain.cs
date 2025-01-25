using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record Domain
{
    public required string Host { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DomainApp App { get; init; }
} 