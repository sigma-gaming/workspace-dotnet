using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Core.Services.Auth;

public static class HttpContextExtensions
{
    public static Guid? TryGetUserId(this HttpContext httpContext)
    {
        if (httpContext.User?.Identity?.IsAuthenticated != true)
            return null;

        var claim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim is null)
            return null;

        return Guid.TryParse(claim.Value, out var userId) ? userId : null;
    }

    public static Guid GetUserId(this HttpContext httpContext)
    {
        var userId = httpContext.TryGetUserId();

        if (userId is null)
            throw new InvalidOperationException("User is not authenticated or the claim is missing or invalid");

        return userId.Value;
    }
}