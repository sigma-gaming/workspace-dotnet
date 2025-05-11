using Common.Api.Extensions;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Core.Builder;
using Core.Cache;
using Core.Contracts;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Nats;
using Core.Services;
using Core.Services.System;
using Core.Tasks.Jobs;
using Core.Tasks.Services;
using Hangfire;
using Hangfire.Redis.StackExchange;
using StackExchange.Redis;

const string apiName = "Core Tasks";
var builder = WebApplication.CreateSlimBuilder(args);

builder.AddCommonHttpConfiguration().AddCommonMvcConfiguration();
builder.AddLogging(apiName);

builder.Services.AddHttpClient();

builder.Services.Core()
    .AddCoreDb(builder.Configuration)
    .AddCoreCache(builder.Configuration)
    .AddCoreNats(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddChat()
    .AddPayment(builder.Configuration);

builder.Services.AddScoped<InitializeChatMessagesJob>();
builder.Services.AddScoped<ProcessReferrerPayoutsJob>();
builder.Services.AddScoped<ProcessStaleDepositsJob>();
builder.Services.AddScoped<ProcessStaleWithdrawalsJob>();
builder.Services.AddScoped<SendAdvancedGamesMetricsJob>();
builder.Services.AddScoped<SyncBudgetJob>();
builder.Services.AddScoped<UpdateCurrencyRatesJob>();

builder.Services.AddScoped<IJobsService, JobsService>();

builder.Services.AddHangfire((provider, config) =>
{
    var redis = provider.GetRequiredService<IConnectionMultiplexer>();
    config.UseRedisStorage(redis);
});

builder.Services.AddHangfireServer();

builder.Services.AddHealthChecks()
    .RequireCoreDb()
    .RequireEnabledBackgroundJobs();

var app = builder.Build();

app.MapPublicPort(appBuilder => { appBuilder.UseHangfireDashboard(); });

app.MapInternalPort(appBuilder =>
{
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapLivenessHealthCheck();
        endpoints.MapReadinessHealthCheck();
    });
});

var jobsService = app.Services.GetRequiredService<IJobsService>();
jobsService.RegisterJobs();
jobsService.RunJobsOnStartup();

await app.RunAsync();