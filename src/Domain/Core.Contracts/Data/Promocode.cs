using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record Promocode
{
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public required string Code { get; init; }
    public required int WageringMultiplier { get; init; }
    public required int Usages { get; init; }
    public required int MaxUsages { get; init; }
    public required bool IsActive { get; init; }
    public required string Bonus { get; init; }
    public Guid? UserId { get; init; }
    public string? Campaign { get; init; }
    public Guid? CreatedBy { get; init; }
    public required PromocodeBonusType BonusType { get; init; }
} 