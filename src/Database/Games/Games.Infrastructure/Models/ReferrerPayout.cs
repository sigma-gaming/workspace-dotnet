using System;
using System.Collections.Generic;

namespace Games.Infrastructure.Models;

public partial class ReferrerPayout
{
    public Guid ReferrerId { get; set; }

    public DateTime NextPayoutAt { get; set; }

    public DateTime? LastPayoutAt { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
