using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public partial class PromocodeUsage
{
    public DateTime CreatedAt { get; set; }

    public Guid PromocodeId { get; set; }

    public Guid UserId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public PromocodeUsageStatus Status { get; set; }

    public virtual Promocode Promocode { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
