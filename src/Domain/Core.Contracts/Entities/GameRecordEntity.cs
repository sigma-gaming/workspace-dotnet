using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record GameRecordEntity
{
    public Guid Id { get; set; }
    public Game Game { get; set; }
    public GameOutcome Outcome { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Snapshot { get; set; } = null!;
    public long Bet { get; set; }
    public int Multiplier { get; set; }
    public long Payout { get; set; }
    public string PreviewUserName { get; set; } = null!;
    public Guid UserId { get; set; }
    public Guid? TransactionId { get; set; }
    public ICollection<BalanceEntity> BalanceMaxMultiplierGames { get; set; } = new List<BalanceEntity>();
    public ICollection<BalanceEntity> BalanceMaxWinGames { get; set; } = new List<BalanceEntity>();
    public TransactionEntity? Transaction { get; set; }
    public UserEntity User { get; set; } = null!;
}
