namespace Core.Contracts.Entities;

public sealed record ReferrerSettingsEntity
{
    public Guid ReferrerId { get; set; }
    public int RevShare { get; set; }
    public UserEntity Referrer { get; set; } = null!;
}