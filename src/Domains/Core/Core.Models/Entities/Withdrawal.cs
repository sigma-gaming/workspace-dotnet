using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class Withdrawal
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

    public virtual Transaction? Transaction { get; set; }

    public virtual User User { get; set; } = null!;
}
