using System;
using System.Collections.Generic;

namespace Games.Infrastructure.Models;

public partial class UserStat
{
    public Guid UserId { get; set; }

    public int DepositCount { get; set; }

    public int WithdrawCount { get; set; }

    public virtual User User { get; set; } = null!;
}
