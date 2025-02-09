using TaskStatus = Core.Contracts.Enums.TaskStatus;

namespace Core.Contracts.Compounds;

public sealed record GlobalTaskStatusInfo(
    TaskStatus Status,
    bool IsInserted);