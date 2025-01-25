namespace Core.Contracts.Entities;

public sealed record ReferrerBalanceEntity
{
    public Guid ReferrerId { get; set; }

    public long Available { get; set; }

    public UserEntity Referrer { get; set; } = null!;
}
