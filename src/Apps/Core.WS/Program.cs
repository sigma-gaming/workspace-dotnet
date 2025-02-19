using Common.Api.Extensions;
using Common.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.Builder;
using Core.Contracts;
using Core.Contracts.Enums;
using Core.Infrastructure;
using Core.Services;
using Core.Services.Auth;
using Core.Services.System;
using Core.WS.Configuration;
using Core.WS.Contracts;
using Core.WS.Hubs;
using Core.WS.Services;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateSlimBuilder(args);

builder.AddCommonHttpConfiguration();
builder.AddLogging("Core WS");

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

        options.DocumentFilter<IncludeModelDocumentFilter<Event>>();
    });

builder.Services.AddCors();
builder.Services.AddSignalR();
builder.Services.AddHttpClient();

// Add infrastructure and domains
builder.Services.Core()
    .AddInfrastructure(builder.Configuration)
    .AddMappers()
    .AddCommonServices();

builder.Services.AddHostedService<SystemBroadcastService>();
builder.Services.AddHostedService<UpdatesBroadcastService>();
builder.Services.AddHostedService<ChatBroadcastService>();
builder.Services.AddHostedService<GameHistoryBroadcastService>();

// Add authentication
builder.Services.AddSessionAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseCommonHttpConfiguration();

if (apiOptions.EnableOpenApi) app.MapCommonOpenApi(options => { options.WithTitle("Core WS"); });

app.UseDomainCors([DomainApp.CoreApp], options =>
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

app.UseAuthentication().UseAuthorization();

app.UseRouting();
app.MapHub<MainHub>("/hub");

await app.RunAsync();