using Core.Contracts.Data;
using Core.Services.Promotions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Affiliate.API.Controllers;

[ApiController]
public class AffiliateController(AffiliateService affiliateService) : ControllerBase
{
    [ProducesResponseType<ReferralCampaign>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/campaigns/by-code/{code}")]
    public async Task<Ok<ReferralCampaign>> GetCampaignByCode(string code)
    {
        var campaign = await affiliateService.TryGetCampaignAsync(code);
        return TypedResults.Ok(campaign);
    }

    [HttpPost("/campaigns/{id:guid}/increment-visits")]
    public async Task<Ok> IncrementCampaignVisits(Guid id)
    {
        await affiliateService.IncrementCampaignVisitsAsync(id);
        return TypedResults.Ok();
    }
}