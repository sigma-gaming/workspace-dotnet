namespace Core.Contracts.Entities;

public sealed record ReferralCampaignEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public int TotalVisits { get; set; } = 0;
    public int TotalSignups { get; set; } = 0;
    public Guid? ReferrerId { get; set; }
    public UserEntity? Referrer { get; set; }
    public ICollection<ReferrerTransactionEntity> ReferrerTransactions { get; set; } = new List<ReferrerTransactionEntity>();
    public ICollection<SessionEntity> Sessions { get; set; } = new List<SessionEntity>();
}
