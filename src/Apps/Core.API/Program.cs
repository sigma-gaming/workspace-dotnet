using Common.Api.Extensions;
using Common.Api.RateLimiting;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Core.API.Infrastructure.RateLimiting;
using Core.Builder;
using Core.Cache;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Nats;
using Core.Services;
using Core.Services.Auth;
using Core.Services.System;
using Microsoft.OpenApi.Models;
using Prometheus;
using Scalar.AspNetCore;

const string apiName = "Core API";
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

        // Add additional models
        options.DocumentFilter<IncludeModelDocumentFilter<Currency>>();
    });

builder.Services.AddHttpClient();

builder.Services.Core()
    .WithEnvironment(builder.Environment)
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddCoreNats(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddPubSub()
    .AddSystem()
    .AddChat()
    .AddNotifications()
    .AddTelegram(builder.Configuration)
    .AddGlobalTasks()
    .AddCurrencyRates(builder.Configuration)
    .AddPayment(builder.Configuration);
builder.Services.AddSessionAuth();

builder.Services.AddRateLimiting(options => options
    .WithBypassToken(apiOptions.RateLimitBypassToken)
    .WithGlobalLimit(new TokenBucketRateLimitSettings(200, 50, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Game.Play,
        new TokenBucketRateLimitSettings(20, 10, TimeSpan.FromSeconds(5)))
    .AddPolicy(
        RateLimitPolicies.Chat.Message,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Promocode.Apply,
        new TokenBucketRateLimitSettings(20, 10, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Profile.Update,
        new TokenBucketRateLimitSettings(3, 1, TimeSpan.FromMinutes(1)))
    .AddPolicy(
        RateLimitPolicies.Task.Action,
        new TokenBucketRateLimitSettings(10, 5, TimeSpan.FromMinutes(1)))
    .AddPolicy(
        RateLimitPolicies.Affiliate.Withdraw,
        new TokenBucketRateLimitSettings(3, 1, TimeSpan.FromMinutes(1))));

builder.Services.AddHealthChecks()
    .RequireCoreDb()
    .RequireDisabledMaintenance();

builder.Services.AddMetrics();

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    if (apiOptions.EnableOpenApi) appBuilder.UseCommonOpenApi();
    appBuilder.UseDomainCors([DomainApp.CoreApp], options =>
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
        endpoints.MapMetrics();
    });
});

await app.RunAsync();