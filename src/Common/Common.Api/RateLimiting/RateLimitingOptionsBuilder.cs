using System.Globalization;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Common.Api.RateLimiting;

public sealed class RateLimitingOptionsBuilder
{
    private readonly List<RateLimitPolicySettings> policies = [];
    private string? bypassToken;
    private readonly Func<HttpContext, string> clientIpResolver = DefaultClientIpResolver;
    private RateLimitPolicySettings? globalLimit;
    private readonly Func<OnRejectedContext, CancellationToken, ValueTask> onRejected = DefaultOnRejected;

    private static string DefaultClientIpResolver(HttpContext context)
    {
        var ip = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault()
                 ?? context.Request.Headers["X-Real-IP"].FirstOrDefault()
                 ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault()?.Split(',').FirstOrDefault()?.Trim()
                 ?? context.Connection.RemoteIpAddress?.ToString();

        return ip ?? "unknown";
    }

    private static async ValueTask DefaultOnRejected(OnRejectedContext context, CancellationToken ct)
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "Слишком много запросов. Попробуйте позже.",
        }, ct);
    }

    public RateLimitingOptionsBuilder WithBypassToken(string? token)
    {
        bypassToken = token;
        return this;
    }

    public RateLimitingOptionsBuilder WithGlobalLimit(IRateLimitSettings settings)
    {
        globalLimit = new RateLimitPolicySettings("global", settings);
        return this;
    }

    public RateLimitingOptionsBuilder AddPolicy(string name, IRateLimitSettings settings)
    {
        policies.Add(new RateLimitPolicySettings(name, settings));
        return this;
    }

    internal RateLimitingOptions Build()
    {
        return new RateLimitingOptions
        {
            BypassToken = bypassToken,
            GlobalLimit = globalLimit,
            Policies = policies,
            ClientIpResolver = clientIpResolver,
            OnRejected = onRejected,
        };
    }
}