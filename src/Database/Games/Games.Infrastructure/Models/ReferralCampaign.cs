using System;
using System.Collections.Generic;

namespace Games.Infrastructure.Models;

public partial class ReferralCampaign
{
    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int TotalVisits { get; set; }

    public int TotalSignups { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid Id { get; set; }

    public virtual User? Referrer { get; set; }

    public virtual ICollection<ReferrerTransaction> ReferrerTransactions { get; set; } = new List<ReferrerTransaction>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
}
