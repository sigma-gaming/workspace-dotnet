namespace Core.Contracts.Data;

public sealed record Config
{
    public required int Id { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required bool MaintenanceEnabled { get; init; }
    public required bool BackgroundJobsEnabled { get; init; }
} 