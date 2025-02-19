using Common.Api.Extensions;
using Common.Logging;
using Common.OpenApi.DocumentFilters;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
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

app.UseAuthentication().UseAuthorization();

app.MapControllers();
await app.RunAsync();