using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.Auth;

public static class AuthenticationExtensions
{
    private const string DEFAULT_SCHEME = "SessionScheme";

    public static AuthenticationBuilder AddSessionAuth(
        this IServiceCollection services,
        string schemeName = DEFAULT_SCHEME)
    {
        return services
            .AddAuthorization()
            .AddAuthentication()
            .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(
                schemeName,
                _ => { });
    }

    public static IApplicationBuilder UseSessionAuth(
        this IApplicationBuilder app)
    {
        return app.UseAuthentication().UseAuthorization();
    }
}