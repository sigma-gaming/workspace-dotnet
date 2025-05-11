using Common.Api.Extensions;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.Extensions;
using Control.API.Configuration;
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
using Scalar.AspNetCore;

const string apiName = "Control API";
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
    });

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddCoreNats(builder.Configuration)
    .AddPubSub()
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddSystem()
    .AddNotifications();

builder.Services.AddSessionAuth();

builder.Services.AddHealthChecks()
    .RequireCoreDb();

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    if (apiOptions.EnableOpenApi) appBuilder.UseCommonOpenApi();
    appBuilder.UseDomainCors([DomainApp.ControlApp], options =>
    {
        options.WithHeaders("content-type",
            "sentry-trace",
            "baggage",
            "x-bypass-rate-limit",
            "x-bypass-waf");
    });
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