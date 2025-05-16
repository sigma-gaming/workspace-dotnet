using Common.Api.Extensions;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Core.Builder;
using Core.Cache;
using Core.Cache.HealthChecks;
using Core.Contracts;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Nats;
using Core.Nats.HealthChecks;
using Core.RabbitMq;
using Core.Services;
using Core.Services.System;
using Microsoft.OpenApi.Models;
using Prometheus;
using Scalar.AspNetCore;

const string apiName = "Payment API";
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
    });

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddCoreNats(builder.Configuration)
    .AddCoreRabbitMq(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddPubSub()
    .AddSystem()
    .AddOutbox(builder.Configuration)
    .AddPaymentWebhooks(builder.Configuration);

// RabbitMQ is not required, webhooks are kept in the outbox while it is unavailable
builder.Services.AddHealthChecks()
    .RequireCoreDb()
    .RequireCoreCache()
    .RequireCoreNats()
    .RequireDisabledMaintenance();

var app = builder.Build();

app.MapPublicPort(appBuilder =>
{
    if (apiOptions.EnableOpenApi) appBuilder.UseCommonOpenApi();
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