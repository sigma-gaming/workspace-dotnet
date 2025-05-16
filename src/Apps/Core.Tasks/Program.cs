using Common.Api.Extensions;
using Common.Monitoring.HealthChecks;
using Common.Monitoring.Logging;
using Core.Builder;
using Core.Cache;
using Core.Contracts;
using Core.Db;
using Core.Db.HealthChecks;
using Core.Nats;
using Core.RabbitMq;
using Core.RabbitMq.HealthChecks;
using Core.Services;
using Core.Services.Payment.Webhooks;
using Core.Services.System;
using Core.Tasks.Jobs;
using Core.Tasks.Services;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Prometheus;
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
    .AddCoreRabbitMq(builder.Configuration)
    .AddMappers()
    .AddCommonServices(builder.Configuration)
    .AddCurrencyRates(builder.Configuration)
    .AddSystem()
    .AddPubSub()
    .AddChat()
    .AddPayment(builder.Configuration);

builder.Services.AddScoped<CleanupOutboxJob>();
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

builder.Services.AddHostedService<PaymentWebhookConsumer>();

builder.Services.AddHealthChecks()
    .RequireCoreDb()
    .RequireCoreRabbitMq()
    .RequireEnabledBackgroundJobs();

var app = builder.Build();

app.MapPublicPort(appBuilder => { appBuilder.UseHangfireDashboard(); });

app.MapInternalPort(appBuilder =>
{
    appBuilder.UseEndpoints(endpoints =>
    {
        endpoints.MapLivenessHealthCheck();
        endpoints.MapReadinessHealthCheck();
        endpoints.MapMetrics();
    });
});

using (var scope = app.Services.CreateScope())
{
    var jobsService = scope.ServiceProvider.GetRequiredService<IJobsService>();
    jobsService.RegisterJobs();
    jobsService.RunJobsOnStartup();
}

await app.RunAsync();