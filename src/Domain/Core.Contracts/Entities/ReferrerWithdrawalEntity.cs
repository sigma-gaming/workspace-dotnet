namespace Core.Contracts.Entities;

public sealed record ReferrerWithdrawalEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public long Amount { get; set; }
    public Guid? ReferrerId { get; set; }
    public UserEntity? Referrer { get; set; }
}