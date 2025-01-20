using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record Withdrawal
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long GemAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public Guid UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public Guid Id { get; set; }
    public string ProviderTransactionId { get; set; } = null!;
    public string ProviderAmount { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public Currency Currency { get; set; }
    public WithdrawalMethod Method { get; set; }
    public PaymentProvider Provider { get; set; }
    public Transaction? Transaction { get; set; }
    public User User { get; set; } = null!;
}
