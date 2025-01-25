using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record ChatMessageEntity
{
    public Guid TrackingId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? Text { get; set; }

    public string? Attachments { get; set; }

    public string? SenderName { get; set; }

    public string? SenderUsername { get; set; }

    public string? SenderImage { get; set; }

    public Guid? UserId { get; set; }

    public bool IsPinned { get; set; }

    public int? ProfileId { get; set; }

    public Guid Id { get; set; }

    public ChatMessageType Type { get; set; }

    public UserRole[] SenderRoles { get; set; } = Array.Empty<UserRole>();

    public ProfileEntity? Profile { get; set; }

    public UserEntity? User { get; set; }
}
