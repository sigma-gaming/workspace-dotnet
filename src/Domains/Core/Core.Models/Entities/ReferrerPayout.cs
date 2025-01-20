using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class ReferrerPayout
{
    public Guid ReferrerId { get; set; }

    public DateTime NextPayoutAt { get; set; }

    public DateTime? LastPayoutAt { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
