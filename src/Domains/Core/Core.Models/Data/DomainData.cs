using System;
using Core.Domain.Enums;

namespace Core.Domain.Data;

public sealed record DomainData
{
    public required string Host { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DomainApp App { get; init; }
} 