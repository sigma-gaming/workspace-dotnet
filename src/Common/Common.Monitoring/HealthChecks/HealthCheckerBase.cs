using Common.Monitoring.HealthChecks.Abstractions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Common.Monitoring.HealthChecks;

public class HealthCheckerBase(IHealthChecker checker) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken ct = default)
    {
        try
        {
            var healthy = await checker.CheckAsync(ct);

            return healthy
                ? HealthCheckResult.Healthy($"{checker.Name} is healthy")
                : HealthCheckResult.Unhealthy($"{checker.Name} is unhealthy");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy($"{checker.Name} check failed", ex);
        }
    }
}