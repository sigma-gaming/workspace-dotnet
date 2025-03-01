using Access.API.Infrastructure.RateLimiting;
using Core.Services.Auth;
using Core.Services.Caching;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Access.API.Controllers;

public sealed record ExchangePayload
{
    public required string Code { get; init; }
    public required Uri ReturnUrl { get; init; }
}

public sealed record RefreshResponse
{
    public required DateTime ExpiresAt { get; init; }
}

public sealed record LogoutPayload
{
    public required Uri ReturnUrl { get; init; }
}

[ApiController]
public class SessionController(
    SessionManager sessionManager,
    SessionService sessionService,
    CoreCacheRegistry cache) : ControllerBase
{
    [EnableRateLimiting(RateLimitPolicies.Exchange)]
    [HttpGet("/exchange")]
    public async Task<IResult> Exchange([FromQuery] ExchangePayload payload)
    {
        if (!cache.IsReady()) return TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

        var sessionIdString = await cache.SessionCodeToSessionId.GetAsync(payload.Code);

        if (sessionIdString is null)
            return TypedResults.BadRequest("Код авторизации истёк или не существует, повторите вход");

        var sessionId = Guid.Parse(sessionIdString);

        var session = await sessionService.GetSessionAsync(sessionId);

        if (session is null)
            return TypedResults.BadRequest("Сессия истекла или не существует, повторите вход");

        var hostname = payload.ReturnUrl.Host;
        var domain = SessionManager.GetBaseDomain(HttpContext);

        if (hostname != domain && !hostname.EndsWith("." + domain))
            return TypedResults.BadRequest("Некорретный URL возврата");

        await cache.SessionCodeToSessionId.DeleteAsync(payload.Code);
        sessionManager.AttachSession(HttpContext, new SessionInfo(sessionId, session.ExpiresAt));

        return TypedResults.Redirect(payload.ReturnUrl.ToString());
    }

    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.Refresh)]
    [ProducesResponseType<RefreshResponse>(StatusCodes.Status200OK, "application/json")]
    [HttpPost("/refresh")]
    public async Task<Ok<RefreshResponse>> Refresh()
    {
        var sessionId = HttpContext.GetSessionId();

        var expiresAt = await sessionService.RefreshSessionAsync(sessionId);
        sessionManager.AttachSession(HttpContext, new SessionInfo(sessionId, expiresAt));

        return TypedResults.Ok(new RefreshResponse { ExpiresAt = expiresAt });
    }

    [Authorize]
    [EnableRateLimiting(RateLimitPolicies.Logout)]
    [HttpGet("/logout")]
    public async Task<IResult> Logout([FromQuery] LogoutPayload payload)
    {
        var sessionId = HttpContext.GetSessionId();

        var hostname = payload.ReturnUrl.Host;
        var domain = SessionManager.GetBaseDomain(HttpContext);

        if (hostname != domain && !hostname.EndsWith("." + domain))
            return TypedResults.BadRequest("Некорретный URL возврата");

        await sessionService.RemoveSessionAsync(sessionId);

        sessionManager.DetachSession(HttpContext);
        return TypedResults.Redirect(payload.ReturnUrl.ToString());
    }
}