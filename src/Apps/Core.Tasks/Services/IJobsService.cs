namespace Core.Tasks.Services;

/// <summary>
///     Interface for service that manages background job registration
/// </summary>
public interface IJobsService
{
    /// <summary>
    ///     Registers all background jobs with the scheduler
    /// </summary>
    void RegisterJobs();

    /// <summary>
    ///     Triggers specified jobs to run immediately on application startup
    /// </summary>
    void RunJobsOnStartup();
}