using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Prometheus;

namespace Common.Monitoring.Metrics;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMetricsEndpoint(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/metrics")
    {
        endpoints.MapMetrics(pattern).AllowAnonymous();
        return endpoints;
    }
}