using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Api.RateLimiting;

public static class RateLimitingExtensions
{
    public static IServiceCollection AddRateLimiting(
        this IServiceCollection services,
        Action<RateLimitingOptionsBuilder> configure)
    {
        var builder = new RateLimitingOptionsBuilder();
        configure(builder);
        var options = builder.Build();

        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.OnRejected = options.OnRejected;

            if (options.GlobalLimit is not null)
                ConfigureRateLimiter(rateLimiterOptions, options, options.GlobalLimit, true);

            foreach (var policy in options.Policies)
                ConfigureRateLimiter(rateLimiterOptions, options, policy, false);
        });

        return services;
    }

    private static void ConfigureRateLimiter(
        RateLimiterOptions rateLimiterOptions,
        RateLimitingOptions options,
        RateLimitPolicySettings policy,
        bool isGlobal)
    {
        var partition = (HttpContext context) =>
        {
            var clientIp = options.ClientIpResolver(context);

            if (options.BypassToken is not null &&
                context.Request.Headers.TryGetValue("x-bypass-rate-limit", out var headerValue) &&
                headerValue == options.BypassToken)
                return RateLimitPartition.GetNoLimiter($"{clientIp}-bypass");

            return policy.Settings switch
            {
                TokenBucketRateLimitSettings settings =>
                    RateLimitPartition.GetTokenBucketLimiter(clientIp, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = settings.TokenLimit,
                        TokensPerPeriod = settings.TokensPerPeriod,
                        ReplenishmentPeriod = settings.ReplenishmentPeriod,
                        QueueLimit = 0,
                    }),
                _ => throw new ArgumentException($"Unsupported rate limit settings type: {policy.Settings.GetType()}"),
            };
        };

        if (isGlobal)
            rateLimiterOptions.GlobalLimiter = PartitionedRateLimiter.Create(partition);
        else
            rateLimiterOptions.AddPolicy(policy.Name, partition);
    }
}