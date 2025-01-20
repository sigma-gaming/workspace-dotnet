using System;
using Core.Domain.Enums;

namespace Core.Domain.Data;

public sealed record ProfileData
{
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required string Name { get; init; }
    public string? Username { get; init; }
    public required Guid UserId { get; init; }
    public required bool HasCustomName { get; init; }
    public string? Image { get; init; }
    public required int Id { get; init; }
    public required AccountProvider UsedProvider { get; init; }
} 