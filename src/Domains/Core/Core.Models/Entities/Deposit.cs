using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class Deposit
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public long GemAmount { get; set; }

    public decimal CurrencyAmount { get; set; }

    public string Payload { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid? TransactionId { get; set; }

    public Guid Id { get; set; }

    public string ProviderTransactionId { get; set; } = null!;

    public string ProviderAmount { get; set; } = null!;

    public PaymentStatus Status { get; set; }

    public Currency Currency { get; set; }

    public DepositMethod Method { get; set; }

    public PaymentProvider Provider { get; set; }

    public DepositType Type { get; set; }

    public virtual Transaction? Transaction { get; set; }

    public virtual User User { get; set; } = null!;
}
