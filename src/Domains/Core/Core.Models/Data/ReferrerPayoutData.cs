using System;

namespace Core.Domain.Data;

public sealed record ReferrerPayoutData
{
    public required Guid ReferrerId { get; init; }
    public required DateTime NextPayoutAt { get; init; }
    public DateTime? LastPayoutAt { get; init; }
} 