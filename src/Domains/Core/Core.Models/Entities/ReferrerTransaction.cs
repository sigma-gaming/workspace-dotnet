using System;
using System.Collections.Generic;
using Core.Domain.Enums;

namespace Core.Domain.Entities;

public partial class ReferrerTransaction
{
    public DateTime CreatedAt { get; set; }

    public long Amount { get; set; }

    public Guid ReferrerId { get; set; }

    public bool IsProcessed { get; set; }

    public Guid? ReferralId { get; set; }

    public Guid? ReferralCampaignId { get; set; }

    public Guid Id { get; set; }

    public ReferralAction ReferralAction { get; set; }

    public virtual User? Referral { get; set; }

    public virtual ReferralCampaign? ReferralCampaign { get; set; }

    public virtual User Referrer { get; set; } = null!;
}
