namespace Common.Monitoring.HealthChecks.Abstractions;

public interface IHealthChecker
{
    string Name { get; }
    Task<bool> CheckAsync(CancellationToken ct = default);
}