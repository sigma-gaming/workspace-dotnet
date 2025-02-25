using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;

namespace Common.Monitoring.HealthChecks;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapLivenessHealthCheck(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/healthy")
    {
        endpoints.MapHealthChecks(pattern, new HealthCheckOptions
        {
            Predicate = _ => false,
        }).AllowAnonymous();

        return endpoints;
    }

    public static IEndpointRouteBuilder MapReadinessHealthCheck(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/ready")
    {
        endpoints.MapHealthChecks(pattern, new HealthCheckOptions
        {
            Predicate = registration => registration.Tags.Contains("ready"),
        }).AllowAnonymous();

        return endpoints;
    }
}