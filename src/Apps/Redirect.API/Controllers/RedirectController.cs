using Core.Contracts.Enums;
using Core.Services.Promotions;
using Core.Services.System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Redirect.API.Infrastructure.RateLimiting;

namespace Redirect.API.Controllers;

[ApiController]
public class RedirectController(DomainService domainService, AffiliateService affiliateService) : ControllerBase
{
    [EnableRateLimiting(RateLimitPolicies.Redirect)]
    [HttpGet("/r/{code}")]
    public async Task<Results<RedirectHttpResult, StatusCodeHttpResult>> RedirectByCode(string code)
    {
        var campaign = await affiliateService.TryGetCampaignAsync(code);

        if (campaign is not null)
            await affiliateService.IncrementCampaignVisitsAsync(campaign.Id);

        var domain = domainService.GetLatestDomain(DomainApp.CoreApp);

        if (domain is null)
            return TypedResults.StatusCode(StatusCodes.Status503ServiceUnavailable);

        return TypedResults.Redirect($"https://{domain.Host}/?r={code}");
    }
}