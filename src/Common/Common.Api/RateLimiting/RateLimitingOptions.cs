using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;

namespace Common.Api.RateLimiting;

public sealed record RateLimitingOptions
{
    public string? BypassToken { get; init; }
    public RateLimitPolicySettings? GlobalLimit { get; init; }

    public IReadOnlyCollection<RateLimitPolicySettings> Policies { get; init; } =
        Array.Empty<RateLimitPolicySettings>();

    public required Func<HttpContext, string> ClientIpResolver { get; init; }
    public required Func<OnRejectedContext, CancellationToken, ValueTask> OnRejected { get; init; }
}