using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class Balance
{
    public Guid UserId { get; set; }

    public long Available { get; set; }

    public long WageringRequired { get; set; }

    public long TotalBet { get; set; }

    public long TotalWin { get; set; }

    public long TotalLost { get; set; }

    public int TotalBetCount { get; set; }

    public long MaxWin { get; set; }

    public int MaxMultiplier { get; set; }

    public Guid? MaxWinGameId { get; set; }

    public Guid? MaxMultiplierGameId { get; set; }

    public virtual GameRecord? MaxMultiplierGame { get; set; }

    public virtual GameRecord? MaxWinGame { get; set; }

    public virtual User User { get; set; } = null!;
}
