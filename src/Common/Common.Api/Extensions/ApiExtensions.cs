using System.Net;
using System.Text.Json;
using Common.Api.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Api.Extensions;

public static class ApiExtensions
{
    public static WebApplicationBuilder AddCommonHttpConfiguration(
        this WebApplicationBuilder builder, Action<HttpOptions>? configure = null)
    {
        var httpOptions = new HttpOptions();
        builder.Configuration.Bind("Http", httpOptions);
        configure?.Invoke(httpOptions);
        builder.Services.AddSingleton(httpOptions);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.Http2.MaxStreamsPerConnection = 1000;
            options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
            options.Limits.MinRequestBodyDataRate = null;
            options.AddServerHeader = false;

            var ip = builder.Environment.IsDevelopment() ? IPAddress.Loopback : IPAddress.Any;

            options.Listen(ip, httpOptions.PublicPort, listenOptions =>
            {
                if (builder.Environment.IsDevelopment()) listenOptions.UseHttps();
            });

            options.Listen(ip, httpOptions.InternalPort, listenOptions =>
            {
                if (builder.Environment.IsDevelopment()) listenOptions.UseHttps();
            });
        });

        // if (builder.Environment.IsDevelopment()) builder.WebHost.UseKestrelHttpsConfiguration();

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

    public static WebApplication MapPublicPort(
        this WebApplication app,
        Action<IApplicationBuilder> configure)
    {
        var httpOptions = app.Services.GetRequiredService<HttpOptions>();
        app.MapWhen(context => context.Connection.LocalPort == httpOptions.PublicPort, builder =>
        {
            builder.UseRouting();
            configure(builder);
        });
        return app;
    }

    public static WebApplication MapInternalPort(
        this WebApplication app,
        Action<IApplicationBuilder> configure)
    {
        var httpOptions = app.Services.GetRequiredService<HttpOptions>();
        app.MapWhen(context => context.Connection.LocalPort == httpOptions.InternalPort, builder =>
        {
            builder.UseRouting();
            configure(builder);
        });
        return app;
    }
}