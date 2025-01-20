using Core.Infrastructure;
using Core.Domain.Services;
using Common.OpenApi.Extensions;
using Core.API.Configuration;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var apiOptions = new ApiOptions();
builder.Configuration.Bind("Api", apiOptions);

if (apiOptions.EnableOpenApi)
{
    builder.Services.AddCommonOpenApi();
}

// Add services to the container.
builder.Services.AddControllers();

// Add Core infrastructure
builder.Services.AddCoreInfrastructure(builder.Configuration).AddCoreDomain();

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

app.UseAuthorization();
app.MapControllers();

await app.RunAsync();