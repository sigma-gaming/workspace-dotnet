using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public class GlobalTask
{
    public GlobalTaskKey Key { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public string Requirements { get; set; } = null!;

    public long Payout { get; set; }

    public int WageringMultiplier { get; set; }
}
