using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record Profile
{
    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Username { get; set; }

    public Guid UserId { get; set; }

    public bool HasCustomName { get; set; }

    public string? Image { get; set; }

    public int Id { get; set; }

    public AccountProvider UsedProvider { get; set; }

    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    public User User { get; set; } = null!;
}
