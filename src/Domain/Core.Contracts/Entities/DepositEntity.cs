using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Contracts.Entities;

public sealed record DepositEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public long GemAmount { get; set; }
    public decimal CurrencyAmount { get; set; }
    public DepositPayload Payload { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public string ProviderTransactionId { get; set; } = null!;
    public string ProviderAmount { get; set; } = null!;
    public PaymentStatus Status { get; set; }
    public Currency Currency { get; set; }
    public DepositMethod Method { get; set; }
    public PaymentProvider Provider { get; set; }
    public DepositType Type { get; set; }
    public TransactionEntity? Transaction { get; set; }
    public UserEntity User { get; set; } = null!;
}
