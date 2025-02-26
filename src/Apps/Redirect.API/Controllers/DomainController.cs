using Core.Contracts.Enums;
using Core.Services.System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Redirect.API.Infrastructure.RateLimiting;

namespace Redirect.API.Controllers;

[ApiController]
public class DomainController(DomainService domainService) : ControllerBase
{
    [EnableRateLimiting(RateLimitPolicies.Domains)]
    [HttpGet("/domains")]
    public Ok<string[]> GetDomains()
    {
        var domains = domainService.GetDomains(DomainApp.CoreApp).ToArray();
        return TypedResults.Ok(domains.Select(x => x.Host).ToArray());
    }
}