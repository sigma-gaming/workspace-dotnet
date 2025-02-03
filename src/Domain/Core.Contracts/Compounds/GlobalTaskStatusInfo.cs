namespace Core.Contracts.Compounds;

public sealed record GlobalTaskStatusInfo(
    Contracts.Enums.TaskStatus Status,
    bool IsInserted);