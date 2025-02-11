using Core.Contracts.Enums;
using Core.Contracts.Jsonb;

namespace Core.Contracts.Entities;

public sealed record ChatMessageEntity
{
    public Guid Id { get; set; }
    public Guid TrackingId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Text { get; set; }
    public ChatMessageAttachment[] Attachments { get; set; } = [];
    public string? SenderName { get; set; }
    public string? SenderUsername { get; set; }
    public string? SenderImage { get; set; }
    public Guid? UserId { get; set; }
    public bool IsPinned { get; set; } = false;
    public int? ProfileId { get; set; }
    public ChatMessageType Type { get; set; }
    public UserRole[] SenderRoles { get; set; } = [];
    public ProfileEntity? Profile { get; set; }
    public UserEntity? User { get; set; }
}