using Access.API.Configuration;
using Access.API.Infrastructure.RateLimiting;
using Common.Api.Extensions;
using Common.Api.RateLimiting;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.Builder;
using Core.Cache;
using Core.Cache.HealthChecks;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Services;
using Core.Services.Auth;
using Core.Services.System;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

const string apiName = "Access API";
var builder = WebApplication.CreateSlimBuilder(args);

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

builder.AddCommonHttpConfiguration().AddCommonMvcConfiguration();
builder.AddLogging(apiName);

if (apiOptions.EnableOpenApi)
    builder.Services.AddCommonOpenApi(options =>
    {
        options.AddServer(new OpenApiServer { Url = apiOptions.PublicUrl });
        options.AddServer(new OpenApiServer { Url = "http://localhost:8080" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Authorization header using the Bearer scheme (session id)",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
        });

        options.DocumentFilter<IncludeModelDocumentFilter<AuthenticationAction>>();
    });

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddSystem();

builder.Services.AddSessionAuth();

builder.Services.AddRateLimiting(options => options
    .WithBypassToken(apiOptions.RateLimitBypassToken)
    .WithGlobalLimit(new TokenBucketRateLimitSettings(200, 50, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Exchange,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromMinutes(10))
    ).AddPolicy(
        RateLimitPolicies.Refresh,
        new TokenBucketRateLimitSettings(2, 1, TimeSpan.FromMinutes(30))
    ).AddPolicy(
        RateLimitPolicies.Logout,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromMinutes(10)))
);

builder.Services.AddHealthChecks()
    .AddCoreDbHealthCheck(true)
    .AddCoreCacheHealthCheck()
    .AddMaintenanceHealthCheck(true);

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    if (apiOptions.EnableOpenApi) appBuilder.UseCommonOpenApi();
    appBuilder.UseDomainCors([DomainApp.CoreApp, DomainApp.ControlApp], options =>
    {
        options.WithHeaders("content-type",
            "sentry-trace",
            "baggage",
            "x-bypass-rate-limit",
            "x-bypass-waf");
    });
    appBuilder.UseRateLimiter();
    appBuilder.UseSessionAuth();
    appBuilder.UseEndpoints(endpoints =>
    {
        if (apiOptions.EnableOpenApi) endpoints.MapCommonOpenApi(options => { options.WithTitle(apiName); });
        endpoints.MapControllers();
    });
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