using System;
using Core.Domain.Enums;

namespace Core.Domain.Data;

public sealed record UserData
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public int? ProfileId { get; init; }
    public Guid? SecurityId { get; init; }
    public Guid? ReferrerId { get; init; }
    public Guid? ReferralCampaignId { get; init; }
    public required bool Virtual { get; init; }
    public required UserRole[] Roles { get; init; }
} 