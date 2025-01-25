using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.Auth;

public static class AuthenticationExtensions
{
    private const string DefaultScheme = "SessionScheme";

    public static AuthenticationBuilder AddSessionAuthentication(
        this IServiceCollection services,
        string schemeName = DefaultScheme)
    {
        return services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = schemeName;
                options.DefaultChallengeScheme = schemeName;
            })
            .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(
                schemeName,
                configureOptions => { });
    }
}