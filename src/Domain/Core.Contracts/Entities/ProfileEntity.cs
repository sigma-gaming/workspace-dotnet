using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record ProfileEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Name { get; set; } = null!;
    public string? Username { get; set; }
    public Guid UserId { get; set; }
    public bool HasCustomName { get; set; }
    public string? Image { get; set; }
    public AccountProvider UsedProvider { get; set; }
    public ICollection<ChatMessageEntity> ChatMessages { get; set; } = new List<ChatMessageEntity>();
    public UserEntity User { get; set; } = null!;
}
