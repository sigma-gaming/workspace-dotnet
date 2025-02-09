using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record ChatMessage
{
    public required Guid TrackingId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public string? Text { get; init; }
    public string? Attachments { get; init; }
    public string? SenderName { get; init; }
    public string? SenderUsername { get; init; }
    public string? SenderImage { get; init; }
    public Guid? UserId { get; init; }
    public required bool IsPinned { get; init; }
    public int? ProfileId { get; init; }
    public required Guid Id { get; init; }
    public required ChatMessageType Type { get; init; }
    public required UserRole[] SenderRoles { get; init; }
}