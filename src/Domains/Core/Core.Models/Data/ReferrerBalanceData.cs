using System;

namespace Core.Domain.Data;

public sealed record ReferrerBalanceData
{
    public required Guid ReferrerId { get; init; }
    public required long Available { get; init; }
} 