using Core.Contracts.Enums;
using Enums_TaskStatus = Core.Contracts.Enums.TaskStatus;
using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.Contracts.Entities;

public sealed record GlobalTaskStatus
{
    public GlobalTaskKey TaskKey { get; set; }
    public Enums_TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
