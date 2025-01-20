using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record Domain
{
    public string Host { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DomainApp App { get; set; }
}
