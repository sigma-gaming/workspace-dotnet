using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record NotificationEntity
{
    public NotificationKind Kind { get; set; }

    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public bool AutoClose { get; set; }

    public int AutoCloseMs { get; set; }

    public bool WithCloseButton { get; set; }

    public Guid? UserId { get; set; }

    public UserEntity? User { get; set; }
}
