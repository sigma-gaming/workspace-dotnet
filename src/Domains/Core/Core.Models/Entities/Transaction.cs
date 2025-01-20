using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record Transaction
{
    public DateTime CreatedAt { get; set; }
    public long Amount { get; set; }
    public Guid? UserId { get; set; }
    public Guid? GameRecordId { get; set; }
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    public Game? Game { get; set; }
    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
    public ICollection<GameRecord> GameRecords { get; set; } = new List<GameRecord>();
    public User? User { get; set; }
    public ICollection<Withdrawal> Withdrawals { get; set; } = new List<Withdrawal>();
}
