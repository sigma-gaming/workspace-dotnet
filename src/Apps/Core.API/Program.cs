using System.Text.Json;
using Common.Logging;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Core.Builder;
using Core.Contracts;
using Core.Infrastructure;
using Core.Services;
using Core.Services.Auth;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.Http2.MaxStreamsPerConnection = 1000;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
    options.Limits.MinRequestBodyDataRate = null;
    options.AddServerHeader = false;
});

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
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "UUID",
        });
    });

builder.Services.AddCors(options => { options.AddDefaultPolicy(policy => { policy.AllowAnyOrigin(); }); });

builder.Services.AddControllers(options =>
{
    // Use camelCase for validation errors serialization
    options.ModelMetadataDetailsProviders.Add(
        new SystemTextJsonValidationMetadataProvider(JsonNamingPolicy.CamelCase));
}).AddJsonOptions(options =>
{
    // Hide detailed serialization error messages in production
    options.AllowInputFormatterExceptionMessages = builder.Environment.IsDevelopment();
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
});

builder.Services.AddHttpClient();

// Add infrastructure and domains
builder.Services.Core()
    .AddInfrastructure(builder.Configuration)
    .AddMappers()
    .AddCommonServices();

// Add authentication
builder.Services.AddSessionAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();

if (apiOptions.EnableOpenApi) app.MapCommonOpenApi(options => { options.WithTitle("Core API"); });

app.UseCors();
app.UseAuthentication().UseAuthorization();

app.MapControllers();
await app.RunAsync();