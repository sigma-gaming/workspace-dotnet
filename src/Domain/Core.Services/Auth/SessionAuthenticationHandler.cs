using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Core.Services.Auth;

public class SessionAuthenticationOptions : AuthenticationSchemeOptions
{
}

public class SessionAuthenticationHandler(
    SessionService sessionService,
    IOptionsMonitor<SessionAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<SessionAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var sessionId = ExtractSessionIdFromRequest();

        if (sessionId is null) return AuthenticateResult.NoResult();

        var session = await sessionService.GetSessionAsync(sessionId.Value);

        if (session is null) return AuthenticateResult.Fail("Session is invalid or expired");

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.UserId.ToString()),
        };

        foreach (var role in session.Roles) claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);

        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }

    private Guid? ExtractSessionIdFromRequest()
    {
        var authHeader = Request.Headers["Authorization"].ToString();

        if (!string.IsNullOrWhiteSpace(authHeader))
            if (AuthenticationHeaderValue.TryParse(authHeader, out var headerVal))
                return Guid.TryParse(headerVal.Parameter, out var headerSessionId)
                    ? headerSessionId
                    : null;

        return Request.Cookies.ContainsKey("session_id")
            ? Guid.TryParse(Request.Cookies["session_id"], out var cookieSessionId) ? cookieSessionId : null
            : null;
    }
}