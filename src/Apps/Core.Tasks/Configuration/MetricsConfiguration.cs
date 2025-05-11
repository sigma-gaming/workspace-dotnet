using Core.Tasks.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tasks.Configuration;

/// <summary>
///     Extension methods for metrics configuration
/// </summary>
public static class MetricsConfiguration
{
    /// <summary>
    ///     Adds metrics configuration to the service collection
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddMetricsConfiguration(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MetricsOptions>(
            configuration.GetSection("Metrics"));

        return services;
    }
}
