using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public class GlobalTask
{
    public GlobalTaskKey Key { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public string Requirements { get; set; } = null!;

    public long Payout { get; set; }

    public int WageringMultiplier { get; set; }
}
