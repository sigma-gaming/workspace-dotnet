using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record AccountData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string ProviderUserId { get; init; }
    public string? ProviderUsername { get; init; }
    public required string ProviderUserFirstName { get; init; }
    public string? ProviderUserLastName { get; init; }
    public string? ProviderUserImage { get; init; }
    public required Guid UserId { get; init; }
    public required AccountProvider Provider { get; init; }
} 