namespace Core.Models.Entities;

public sealed record ReferrerPayout
{
    public Guid ReferrerId { get; set; }

    public DateTime NextPayoutAt { get; set; }

    public DateTime? LastPayoutAt { get; set; }

    public User Referrer { get; set; } = null!;
}
