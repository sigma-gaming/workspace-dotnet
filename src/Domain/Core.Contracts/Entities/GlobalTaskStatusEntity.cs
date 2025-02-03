using Core.Contracts.Enums;

namespace Core.Contracts.Entities;

public sealed record GlobalTaskStatusEntity
{
    public GlobalTaskKey TaskKey { get; set; }
    public Enums.TaskStatus Status { get; set; }
    public Guid UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}