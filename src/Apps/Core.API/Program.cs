using Common.Logging;
using Core.Infrastructure;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Core.Services;
using Scalar.AspNetCore;
using Core.Contracts;
using Core.Services.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging("Core API");

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

if (apiOptions.EnableOpenApi)
{
    builder.Services.AddCommonOpenApi();
}

// Add services to the container.
builder.Services.AddControllers();

// Add infrastructure and domains
builder.Services
    .AddHttpClient()
    .AddCoreInfrastructure(builder.Configuration)
    .AddCoreDomain()
    .AddCoreMappers();

// Add authentication
builder.Services.AddSessionAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (apiOptions.EnableOpenApi)
{
    app.MapCommonOpenApi(options =>
    {
        options.WithTitle("Core API");
        options.Servers = new List<ScalarServer>([new ScalarServer(apiOptions.PublicUrl)]);
    });
}

app.UseAuthentication().UseAuthorization();

app.MapControllers();

await app.RunAsync();