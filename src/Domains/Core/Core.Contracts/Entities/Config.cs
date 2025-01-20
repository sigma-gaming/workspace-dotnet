namespace Core.Contracts.Entities;

public sealed record Config
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public bool MaintenanceEnabled { get; set; }

    public bool BackgroundJobsEnabled { get; set; }
}
