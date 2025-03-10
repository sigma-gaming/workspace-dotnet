using Core.Contracts.Enums;
using Core.Services.System;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Domain.API.Controllers;

public sealed record GetLatestDomainPayload
{
    public required DomainApp App { get; init; }
}

public sealed record GetDomainsPayload
{
    public DomainApp? App { get; init; }
}

[ApiController]
public class DomainController(DomainService domainService) : ControllerBase
{
    [ProducesResponseType<Core.Contracts.Data.Domain[]>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/domains")]
    public Ok<Core.Contracts.Data.Domain[]> GetDomains([FromQuery] GetDomainsPayload payload)
    {
        var apps = payload.App is null ? [] : new[] { payload.App.Value };
        var domains = domainService.GetDomains(apps).ToArray();
        return TypedResults.Ok(domains);
    }

    [ProducesResponseType<Core.Contracts.Data.Domain>(StatusCodes.Status200OK, "application/json")]
    [HttpGet("/domains/latest")]
    public Results<Ok<Core.Contracts.Data.Domain>, NotFound> GetLatestDomain([FromQuery] GetLatestDomainPayload payload)
    {
        var domain = domainService.GetLatestDomain(payload.App);
        if (domain is null) return TypedResults.NotFound();
        return TypedResults.Ok(domain);
    }
}