using Core.Contracts.Enums;

namespace Core.Contracts.Data;

public sealed record NotificationData
{
    public required NotificationKind Kind { get; init; }
    public required Guid Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime ExpiresAt { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public required bool AutoClose { get; init; }
    public required int AutoCloseMs { get; init; }
    public required bool WithCloseButton { get; init; }
    public Guid? UserId { get; init; }
} 