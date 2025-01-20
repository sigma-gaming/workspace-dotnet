namespace Core.Contracts.Entities;

public sealed record ReferrerWithdrawal
{
    public DateTime CreatedAt { get; set; }

    public long Amount { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid Id { get; set; }

    public User? Referrer { get; set; }
}
