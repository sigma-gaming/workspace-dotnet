namespace Common.Api.RateLimiting;

public sealed record TokenBucketSettings(int TokenLimit, int TokensPerPeriod, TimeSpan ReplenishmentPeriod);

public sealed record RateLimitPolicySettings(string Name, IRateLimitSettings Settings);

public interface IRateLimitSettings
{
}

public sealed record TokenBucketRateLimitSettings(
    int TokenLimit,
    int TokensPerPeriod,
    TimeSpan ReplenishmentPeriod) : IRateLimitSettings;