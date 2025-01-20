namespace Core.Contracts.Entities;

public sealed record ReferralCampaign
{
    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int TotalVisits { get; set; }

    public int TotalSignups { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid Id { get; set; }

    public User? Referrer { get; set; }

    public ICollection<ReferrerTransaction> ReferrerTransactions { get; set; } = new List<ReferrerTransaction>();

    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
