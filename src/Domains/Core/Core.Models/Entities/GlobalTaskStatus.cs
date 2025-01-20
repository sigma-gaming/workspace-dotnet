using Core.Domain.Enums;

namespace Core.Models.Entities;

public sealed record GlobalTaskStatus
{
    public GlobalTaskKey TaskKey { get; set; }
    public Core.Domain.Enums.TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
