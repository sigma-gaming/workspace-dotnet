using Common.Api.Extensions;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.Builder;
using Core.Cache;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Db;
using Core.Nats;
using Core.Services;
using Core.Services.Auth;
using Core.Services.System;
using Core.WS.Configuration;
using Core.WS.Contracts;
using Core.WS.Hubs;
using Core.WS.Services;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

const string apiName = "Core WS";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddCommonHttpConfiguration();
builder.AddLogging(apiName);

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

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

        options.DocumentFilter<IncludeModelDocumentFilter<Event>>();
    });

builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddCoreNats(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddPubSub()
    .AddSystem();

builder.Services.AddHostedService<SystemBroadcastService>();
builder.Services.AddHostedService<UpdatesBroadcastService>();
builder.Services.AddHostedService<ChatBroadcastService>();
builder.Services.AddHostedService<GameHistoryBroadcastService>();

builder.Services.AddSessionAuth();
builder.Services.AddAuthorization();

builder.Services.AddHealthChecks().AddMaintenanceHealthCheck(true);

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
            "x-bypass-waf",
            "x-requested-with",
            "x-signalr-user-agent"
        );
    });

    appBuilder.UseAuthentication().UseAuthorization();

    appBuilder.UseEndpoints(endpoints =>
    {
        if (apiOptions.EnableOpenApi) endpoints.MapCommonOpenApi(options => { options.WithTitle(apiName); });
        endpoints.MapHub<MainHub>("/hub");
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