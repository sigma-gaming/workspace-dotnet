using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class ReferrerSettings
{
    public Guid ReferrerId { get; set; }

    public int RevShare { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
