// using Common.OpenApi.Transformers;
// using Microsoft.AspNetCore.OpenApi;

using Common.OpenApi.DocumentFilters;
using Microsoft.AspNetCore.Builder;
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

    public static WebApplication MapCommonOpenApi(
        this WebApplication app,
        Action<ScalarOptions>? configureOptions = null)
    {
        app.MapSwagger();
        app.UseSwagger(options => { options.RouteTemplate = "/openapi/{documentName}.json"; });
        app.MapScalarApiReference(options => { configureOptions?.Invoke(options); });

        return app;
    }
}