using System;

namespace Core.Domain.Data;

public sealed record BudgetData
{
    public required int Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required DateTime LastSyncAt { get; init; }
    public required long Available { get; init; }
} 