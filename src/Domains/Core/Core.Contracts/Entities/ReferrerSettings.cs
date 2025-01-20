namespace Core.Contracts.Entities;

public sealed record ReferrerSettings
{
    public Guid ReferrerId { get; set; }

    public int RevShare { get; set; }

    public User Referrer { get; set; } = null!;
}
