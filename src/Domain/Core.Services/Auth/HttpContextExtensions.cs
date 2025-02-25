using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Services.Auth;

public static class HttpContextExtensions
{
    private static string? TryGetClaim(this HttpContext httpContext, string claimType)
    {
        if (httpContext.User.Identity?.IsAuthenticated != true)
            return null;

        var claim = httpContext.User.FindFirst(claimType);
        return claim?.Value;
    }

    public static Guid? TryGetSessionId(this HttpContext httpContext)
    {
        return Guid.TryParse(httpContext.TryGetClaim(ClaimTypes.Sid), out var sessionId) ? sessionId : null;
    }

    public static Guid? TryGetUserId(this HttpContext httpContext)
    {
        return Guid.TryParse(httpContext.TryGetClaim(ClaimTypes.NameIdentifier), out var userId) ? userId : null;
    }

    public static Guid GetSessionId(this HttpContext httpContext)
    {
        var sessionId = httpContext.TryGetSessionId();

        if (sessionId is null)
            throw new InvalidOperationException("User is not authenticated or the claim is missing or invalid");

        return sessionId.Value;
    }

    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.TryGetUserId();

        if (userId is null)
            throw new InvalidOperationException("User is not authenticated or the claim is missing or invalid");

        return userId.Value;
    }
}