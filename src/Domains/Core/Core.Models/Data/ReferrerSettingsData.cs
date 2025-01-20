using System;

namespace Core.Domain.Data;

public sealed record ReferrerSettingsData
{
    public required Guid ReferrerId { get; init; }
    public required int RevShare { get; init; }
} 