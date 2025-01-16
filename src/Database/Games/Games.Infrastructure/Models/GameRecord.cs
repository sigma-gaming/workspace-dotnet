using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public class GameRecord
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
    public virtual ICollection<Balance> BalanceMaxMultiplierGames { get; set; } = new List<Balance>();
    public virtual ICollection<Balance> BalanceMaxWinGames { get; set; } = new List<Balance>();
    public virtual Transaction? Transaction { get; set; }
    public virtual User User { get; set; } = null!;
}
