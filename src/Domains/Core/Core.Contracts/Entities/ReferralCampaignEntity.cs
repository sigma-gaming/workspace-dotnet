namespace Core.Contracts.Entities;

public sealed record ReferralCampaignEntity
{
    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int TotalVisits { get; set; }

    public int TotalSignups { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid Id { get; set; }

    public UserEntity? Referrer { get; set; }

    public ICollection<ReferrerTransactionEntity> ReferrerTransactions { get; set; } = new List<ReferrerTransactionEntity>();

    public ICollection<SessionEntity> Sessions { get; set; } = new List<SessionEntity>();
}
