using Core.Contracts.Data;
using Core.Contracts.Entities;
using Core.Contracts.Enums;
using Core.Contracts.Mappers;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Core.Services.System;

public sealed class DomainService(
    IDbContextFactory<CoreDbContext> dbContextFactory,
    ILogger<DomainService> logger,
    DomainMapper mapper)
    : BackgroundService
{
    private readonly List<Domain> cached = [];
    private readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(5);
    private readonly TimeSpan retryDelay = TimeSpan.FromSeconds(5);

    private async Task<List<DomainEntity>> QueryDomainsAsync(CancellationToken ct = default)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync(ct);

        return await db.Domains
            .OrderByDescending(d => d.CreatedAt)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    private async Task RefreshDomainsAsync()
    {
        try
        {
            var domains = await QueryDomainsAsync();
            cached.Clear();
            cached.AddRange(domains.Select(mapper.ToData));
            logger.LogInformation("Refreshed domains");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh domains");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
            try
            {
                await RefreshDomainsAsync();
                await Task.Delay(refreshInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Domain refresh cancelled");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to refresh domains, retrying in {Delay} seconds...",
                    retryDelay.TotalSeconds);
                await Task.Delay(retryDelay, stoppingToken);
            }
    }

    public IReadOnlyList<Domain> GetDomains(params DomainApp[]? apps)
    {
        if (apps is null || apps.Length == 0)
            return cached;

        return cached.Where(d => apps.Contains(d.App)).ToList();
    }

    public Domain? GetLatestDomain(DomainApp app)
    {
        return cached.FirstOrDefault(d => d.App == app);
    }

    public IReadOnlyList<Domain> GetDomainsByApp(DomainApp app)
    {
        return cached.Where(d => d.App == app).ToList();
    }

    public bool OriginMatches(string? origin, params DomainApp[]? apps)
    {
        if (string.IsNullOrEmpty(origin))
            return false;

        var domains = GetDomains(apps);
        var host = origin.Replace("https://", "", StringComparison.OrdinalIgnoreCase);

        return domains.Any(d => string.Equals(host, d.Host, StringComparison.OrdinalIgnoreCase));
    }
}