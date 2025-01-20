using System;
using Core.Domain.Enums;

namespace Core.Domain.Data;

public sealed record PromocodeUsageData
{
    public required DateTime CreatedAt { get; init; }
    public required Guid PromocodeId { get; init; }
    public required Guid UserId { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public required PromocodeUsageStatus Status { get; init; }
} 