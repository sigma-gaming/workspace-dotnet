using System;
using System.Collections.Generic;

namespace Core.Domain.Entities;

public partial class ReferrerWithdrawal
{
    public DateTime CreatedAt { get; set; }

    public long Amount { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid Id { get; set; }

    public virtual User? Referrer { get; set; }
}
