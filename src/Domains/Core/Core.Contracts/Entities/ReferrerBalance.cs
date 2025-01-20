namespace Core.Contracts.Entities;

public sealed record ReferrerBalance
{
    public Guid ReferrerId { get; set; }

    public long Available { get; set; }

    public User Referrer { get; set; } = null!;
}
