using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Core.Services.Auth;

public class SessionManager(IOptions<SessionOptions> options)
{
    public static string GetBaseDomain(HttpContext context)
    {
        var host = context.Request.Host.Host;
        var parts = host.Split('.');
        return string.Join(".", parts[^2..]);
    }

    public void AttachSession(HttpContext context, SessionInfo session)
    {
        var domain = GetBaseDomain(context);
        var expires = new DateTimeOffset(session.ExpiresAt);

        context.Response.Cookies.Append(options.Value.SessionIdKey, session.Id.ToString(), new CookieOptions
        {
            Domain = domain,
            Path = "/",
            Expires = expires,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Secure = true,
        });

        context.Response.Cookies.Append(options.Value.SessionExpiresKey, session.ExpiresAt.ToString("O"),
            new CookieOptions
            {
                Domain = domain,
                Path = "/",
                Expires = expires,
                SameSite = SameSiteMode.Lax,
                Secure = true,
            });
    }

    public void DetachSession(HttpContext context)
    {
        var domain = GetBaseDomain(context);

        context.Response.Cookies.Delete(options.Value.SessionIdKey, new CookieOptions
        {
            Domain = domain,
            Path = "/",
            SameSite = SameSiteMode.Lax,
            HttpOnly = true,
        });

        context.Response.Cookies.Delete(options.Value.SessionExpiresKey, new CookieOptions
        {
            Domain = domain,
            Path = "/",
            SameSite = SameSiteMode.Lax,
        });
    }
}

public sealed record SessionInfo(Guid Id, DateTime ExpiresAt);