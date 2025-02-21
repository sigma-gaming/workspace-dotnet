using Common.Api.Extensions;
using Common.Api.RateLimiting;
using Common.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Core.API.Infrastructure.RateLimiting;
using Core.Builder;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Infrastructure;
using Core.Services;
using Core.Services.Auth;
using Core.Services.System;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddCommonHttpConfiguration().AddCommonMvcConfiguration();

builder.AddLogging("Core API");

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

if (apiOptions.EnableOpenApi)
    builder.Services.AddCommonOpenApi(options =>
    {
        options.AddServer(new OpenApiServer { Url = apiOptions.PublicUrl });
        options.AddServer(new OpenApiServer { Url = "http://localhost:8080" });
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "Authorization header using the Bearer scheme (session)",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
        });

        // Add additional models
        options.DocumentFilter<IncludeModelDocumentFilter<Currency>>();
    });

builder.Services.AddHttpClient();

// Add infrastructure and domains
builder.Services.Core()
    .AddInfrastructure(builder.Configuration)
    .AddMappers()
    .AddCommonServices()
    .AddTelegram(builder.Configuration)
    .AddGlobalTasks();

// Add authentication
builder.Services.AddSessionAuthentication();
builder.Services.AddAuthorization();

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

var app = builder.Build();
app.UseCommonHttpConfiguration();

if (apiOptions.EnableOpenApi) app.MapCommonOpenApi(options => { options.WithTitle("Core API"); });

app.UseDomainCors([DomainApp.CoreApp], options =>
{
    options.WithHeaders("content-type",
        "sentry-trace",
        "baggage",
        "x-bypass-rate-limit",
        "x-bypass-waf");
});

app.UseRateLimiter();

app.UseAuthentication().UseAuthorization();

app.MapControllers();
await app.RunAsync();