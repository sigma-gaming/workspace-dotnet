using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record TransactionEntity
{
    public DateTime CreatedAt { get; set; }
    public long Amount { get; set; }
    public Guid? UserId { get; set; }
    public Guid? GameRecordId { get; set; }
    public Guid Id { get; set; }
    public TransactionType Type { get; set; }
    public Game? Game { get; set; }
    public ICollection<DepositEntity> Deposits { get; set; } = new List<DepositEntity>();
    public ICollection<GameRecordEntity> GameRecords { get; set; } = new List<GameRecordEntity>();
    public UserEntity? User { get; set; }
    public ICollection<WithdrawalEntity> Withdrawals { get; set; } = new List<WithdrawalEntity>();
}
