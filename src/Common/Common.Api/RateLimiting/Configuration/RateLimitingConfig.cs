using Microsoft.AspNetCore.Http;

namespace Common.Api.RateLimiting.Configuration;

public sealed record RateLimitingConfig(
    string? BypassToken = null,
    Func<HttpContext, string>? ClientIpResolver = null)
{
    public Func<HttpContext, string> GetClientIpResolver =>
        ClientIpResolver ?? ResolveClientIp;

    private static string ResolveClientIp(HttpContext context)
    {
        var ip = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                 ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                 ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                 ?? context.Connection.RemoteIpAddress?.ToString();

        return ip ?? "unknown";
    }
}