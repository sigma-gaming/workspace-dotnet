using Common.OpenApi.Transformers;
using Microsoft.AspNetCore.Builder;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;

namespace Common.OpenApi.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddCommonOpenApi(
        this IServiceCollection services,
        Action<OpenApiOptions>? configureOptions = null)
    {
        return services.AddOpenApi(options =>
        {
            options.AddSchemaTransformer<EnumSchemaTransformer>();
            configureOptions?.Invoke(options);
        });
    }

    public static IApplicationBuilder MapCommonOpenApi(
        this WebApplication app,
        Action<ScalarOptions>? configureOptions = null)
    {
        app.MapOpenApi();
        app.MapScalarApiReference((options) => { configureOptions?.Invoke(options); });

        return app;
    }
}