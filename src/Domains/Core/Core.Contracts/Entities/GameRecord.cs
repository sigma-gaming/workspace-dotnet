using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record GameRecord
{
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
    public Guid Id { get; set; }
    public ICollection<Balance> BalanceMaxMultiplierGames { get; set; } = new List<Balance>();
    public ICollection<Balance> BalanceMaxWinGames { get; set; } = new List<Balance>();
    public Transaction? Transaction { get; set; }
    public User User { get; set; } = null!;
}
