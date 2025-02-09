using Core.Contracts.Enums;
using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.Contracts.Entities;

public sealed record GlobalTaskStatusEntity
{
    public GlobalTaskKey TaskKey { get; set; }
    public TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}