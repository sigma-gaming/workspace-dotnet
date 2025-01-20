using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class Transaction
{
    public DateTime CreatedAt { get; set; }

    public long Amount { get; set; }

    public Guid? UserId { get; set; }

    public Guid? GameRecordId { get; set; }

    public Guid Id { get; set; }

    public TransactionType Type { get; set; }

    public Game? Game { get; set; }

    public virtual ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();

    public virtual ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();

    public virtual User? User { get; set; }

    public virtual ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
