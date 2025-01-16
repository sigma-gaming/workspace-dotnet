using System;
using System.Collections.Generic;

namespace Games.Infrastructure.Models;

public partial class ReferrerBalance
{
    public Guid ReferrerId { get; set; }

    public long Available { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
