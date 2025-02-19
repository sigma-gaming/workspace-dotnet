using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Extensions;

public static class ApiExtensions
{
    public static WebApplicationBuilder AddCommonHttpConfiguration(
        this WebApplicationBuilder builder)
    {
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.Http2.MaxStreamsPerConnection = 1000;
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
            options.Limits.MinRequestBodyDataRate = null;
            options.AddServerHeader = false;
        });

        if (builder.Environment.IsDevelopment()) builder.WebHost.UseKestrelHttpsConfiguration();

        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        });

        builder.Services.Configure<RouteOptions>(options =>
            options.SetParameterPolicy<RegexInlineRouteConstraint>("regex"));

        return builder;
    }


    public static WebApplicationBuilder AddCommonMvcConfiguration(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options =>
        {
            // Use camelCase for validation errors serialization
            options.ModelMetadataDetailsProviders.Add(
                new SystemTextJsonValidationMetadataProvider(JsonNamingPolicy.CamelCase));
        }).AddJsonOptions(options =>
        {
            // Hide detailed serialization error messages in production
            options.AllowInputFormatterExceptionMessages = builder.Environment.IsDevelopment();
        }).ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });

        return builder;
    }

    public static WebApplication UseCommonHttpConfiguration(
        this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) app.UseHttpsRedirection();

        return app;
    }
}