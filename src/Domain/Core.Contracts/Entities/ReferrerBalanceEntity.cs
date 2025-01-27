namespace Core.Contracts.Entities;

public sealed record ReferrerBalanceEntity
{
    public Guid ReferrerId { get; set; }
    public long Available { get; set; } = 0;
    public UserEntity Referrer { get; set; } = null!;
}
