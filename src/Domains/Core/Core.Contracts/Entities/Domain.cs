using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record Domain
{
    public string Host { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DomainApp App { get; set; }
}
