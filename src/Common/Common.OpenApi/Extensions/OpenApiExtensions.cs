// using Common.OpenApi.Transformers;
// using Microsoft.AspNetCore.OpenApi;

using Common.OpenApi.DocumentFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Common.OpenApi.Extensions;

public static class OpenApiExtensions
{
    // public static IServiceCollection AddCommonOpenApi(
    //     this IServiceCollection services,
    //     Action<OpenApiOptions>? configureOptions = null)
    // {
    //     return services.AddOpenApi(options =>
    //     {
    //         options.AddSchemaTransformer<EnumSchemaTransformer>();
    //         configureOptions?.Invoke(options);
    //     });
    // }

    // public static IApplicationBuilder MapCommonOpenApi(
    //     this WebApplication app,
    //     Action<ScalarOptions>? configureOptions = null)
    // {
    //     app.MapOpenApi();
    //     app.MapScalarApiReference((options) => { configureOptions?.Invoke(options); });
    //
    //     return app;
    // }

    public static IServiceCollection AddCommonOpenApi(
        this IServiceCollection services,
        Action<SwaggerGenOptions>? configureOptions = null)
    {
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();
                options.SupportNonNullableReferenceTypes();
                options.UseOneOfForPolymorphism();
                options.UseAllOfToExtendReferenceSchemas();
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                configureOptions?.Invoke(options);
                options.DocumentFilter<DiscriminatedUnionDocumentFilter>();
                options.DocumentFilter<FlattenAllOfRefDocumentFilter>();
            });

        return services;
    }

    public static IApplicationBuilder UseCommonOpenApi(
        this IApplicationBuilder builder)
    {
        builder.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; });

        return builder;
    }

    public static IEndpointRouteBuilder MapCommonOpenApi(
        this IEndpointRouteBuilder builder,
        Action<ScalarOptions>? configureOptions = null)
    {
        builder.MapSwagger();
        builder.MapScalarApiReference(options => { configureOptions?.Invoke(options); });

        return builder;
    }
}