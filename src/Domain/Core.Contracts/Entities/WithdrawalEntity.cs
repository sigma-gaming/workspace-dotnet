using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record WithdrawalEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public PaymentStatus Status { get; set; }
    public WithdrawalMethod Method { get; set; }
    public PaymentProvider Provider { get; set; }
    public long GemAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public string ProviderAmount { get; set; } = null!;
    public Currency Currency { get; set; }
    public Guid UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public string ProviderTransactionId { get; set; } = null!;
    public TransactionEntity? Transaction { get; set; }
    public UserEntity User { get; set; } = null!;
}