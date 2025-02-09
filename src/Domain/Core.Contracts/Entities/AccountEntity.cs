using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record AccountEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ProviderUserId { get; set; } = null!;
    public string? ProviderUsername { get; set; }
    public string ProviderUserFirstName { get; set; } = null!;
    public string? ProviderUserLastName { get; set; }
    public string? ProviderUserImage { get; set; }
    public Guid UserId { get; set; }
    public AccountProvider Provider { get; set; }
    public UserEntity User { get; set; } = null!;
}