using Core.Contracts.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.System;

public static class CorsExtensions
{
    public static IApplicationBuilder UseDomainCors(
        this IApplicationBuilder app,
        DomainApp[] allowedApps,
        Action<CorsPolicyBuilder>? configureOptions = null)
    {
        var domainService = app.ApplicationServices.GetRequiredService<DomainService>();

        app.UseCors(options =>
        {
            options.SetIsOriginAllowed(origin => domainService.OriginMatches(origin, allowedApps))
                .AllowCredentials()
                .AllowAnyMethod()
                .SetPreflightMaxAge(TimeSpan.FromDays(1));

            configureOptions?.Invoke(options);
        });

        return app;
    }
}