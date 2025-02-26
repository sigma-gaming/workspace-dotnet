using Common.Api.Extensions;
using Common.Api.RateLimiting;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Core.Builder;
using Core.Cache;
using Core.Cache.HealthChecks;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Services;
using Core.Services.System;
using Redirect.API.Configuration;
using Redirect.API.Infrastructure.RateLimiting;

const string apiName = "Redirect API";
var builder = WebApplication.CreateSlimBuilder(args);

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

builder.AddCommonHttpConfiguration().AddCommonMvcConfiguration();
builder.AddLogging(apiName);

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddSystem();

builder.Services.AddRateLimiting(options => options
    .WithGlobalLimit(new TokenBucketRateLimitSettings(200, 50, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Redirect,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromMinutes(5))
    ).AddPolicy(
        RateLimitPolicies.Domains,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromMinutes(1))
    )
);

builder.Services.AddHealthChecks()
    .AddCoreDbHealthCheck(true)
    .AddCoreCacheHealthCheck()
    .AddMaintenanceHealthCheck(true);

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    appBuilder.UseDomainCors([DomainApp.CoreApp], options =>
    {
        options.WithHeaders("content-type",
            "sentry-trace",
            "baggage");
    });
    appBuilder.UseRateLimiter();
    appBuilder.UseEndpoints(endpoints => { endpoints.MapControllers(); });
});

app.MapInternalPort(appBuilder =>
{
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapLivenessHealthCheck();
        endpoints.MapReadinessHealthCheck();
    });
});

await app.RunAsync();