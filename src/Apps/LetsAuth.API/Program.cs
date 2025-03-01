using Common.Api.Extensions;
using Common.Api.RateLimiting;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.Extensions;
using Core.Builder;
using Core.Cache;
using Core.Contracts;
using Core.Db;
using Core.Services;
using Core.Services.Auth;
using LetsAuth.API.Configuration;
using LetsAuth.API.Infrastructure.RateLimiting;
using LetsAuth.API.Integrations;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

const string apiName = "LetsAuth API";
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
            Description = "Authorization header using the Bearer scheme (session)",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Name = "Authorization",
        });
    });

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddTelegram(builder.Configuration)
    .AddVk(builder.Configuration);

builder.Services.AddSingleton<TelegramIntegration>();
builder.Services.AddSingleton<VkIntegration>();

builder.Services.AddSessionAuth();

builder.Services.AddRateLimiting(options => options
    .WithBypassToken(apiOptions.RateLimitBypassToken)
    .WithGlobalLimit(new TokenBucketRateLimitSettings(200, 50, TimeSpan.FromSeconds(10)))
    .AddPolicy(
        RateLimitPolicies.Authenticate,
        new TokenBucketRateLimitSettings(6, 3, TimeSpan.FromMinutes(1))
    )
);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    if (apiOptions.EnableOpenApi) appBuilder.UseCommonOpenApi();
    appBuilder.UseCors(options =>
    {
        options.WithOrigins(apiOptions.ClientUrl)
            .WithHeaders("content-type")
            .AllowCredentials()
            .AllowAnyMethod()
            .SetPreflightMaxAge(TimeSpan.FromDays(1));
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