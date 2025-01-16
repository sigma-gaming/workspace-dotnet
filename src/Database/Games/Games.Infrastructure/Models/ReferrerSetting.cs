using System;
using System.Collections.Generic;

namespace Games.Infrastructure.Models;

public partial class ReferrerSetting
{
    public Guid ReferrerId { get; set; }

    public int RevShare { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
