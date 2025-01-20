using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class UserStats
{
    public Guid UserId { get; set; }

    public int DepositCount { get; set; }

    public int WithdrawCount { get; set; }

    public virtual User User { get; set; } = null!;
}
