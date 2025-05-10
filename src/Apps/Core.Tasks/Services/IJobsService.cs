namespace Core.Tasks.Services;

/// <summary>
/// Interface for service that manages background job registration
/// </summary>
public interface IJobsService
{
    /// <summary>
    /// Registers all background jobs with the scheduler
    /// </summary>
    void RegisterJobs();
}
