namespace Core.Contracts.Entities;

public sealed record ReferrerPayoutEntity
{
    public Guid ReferrerId { get; set; }
    public DateTime NextPayoutAt { get; set; }
    public DateTime? LastPayoutAt { get; set; }
    public UserEntity Referrer { get; set; } = null!;
}
