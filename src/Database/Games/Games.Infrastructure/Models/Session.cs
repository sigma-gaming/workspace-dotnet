using System;
using System.Collections.Generic;
using Games.Domain.Enums;

namespace Games.Infrastructure.Models;

public partial class Session
{
    public DateTime ExpiresAt { get; set; }

    public Guid UserId { get; set; }

    public Guid Id { get; set; }

    public Guid? ReferrerId { get; set; }

    public Guid? ReferralCampaignId { get; set; }

    public AccountProvider Provider { get; set; }

    public virtual ReferralCampaign? ReferralCampaign { get; set; }

    public virtual User? Referrer { get; set; }

    public virtual User User { get; set; } = null!;
}
