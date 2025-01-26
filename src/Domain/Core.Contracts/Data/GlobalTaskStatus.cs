using Core.Contracts.Enums;
using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.Contracts.Data;

public sealed record GlobalTaskStatus
{
    public required GlobalTaskKey TaskKey { get; init; }
    public required TaskStatus Status { get; init; }
    public required Guid UserId { get; init; }
}