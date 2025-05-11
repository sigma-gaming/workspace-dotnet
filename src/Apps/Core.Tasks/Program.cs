using Core.Tasks.Configuration;
using Core.Tasks.Jobs;
using Core.Tasks.Services;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

const string appName = "Core Tasks";

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithProperty("App", appName)
    .MinimumLevel.Information()
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

try
{
    Log.Information("Starting {AppName}", appName);

    // Create and configure the host builder
    var host = Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((hostContext, config) =>
        {
            config.AddJsonFile("appsettings.json", false);
            config.AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);
            config.AddEnvironmentVariables();
        })
        .UseSerilog((hostContext, loggerConfiguration) =>
        {
            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithProperty("App", appName);
            loggerConfiguration.MinimumLevel.Information();

            if (hostContext.HostingEnvironment.IsProduction())
                loggerConfiguration
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Hangfire", LogEventLevel.Information)
                    .WriteTo.Async(c => c.Console(new CompactJsonFormatter()));
            else
                loggerConfiguration
                    .WriteTo.Async(c => c.Console());
        })
        .ConfigureServices((hostContext, services) =>
        {
            var configuration = hostContext.Configuration;

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();
            });

            var workerCount = configuration.GetSection("Hangfire:WorkerCount").Get<int?>() ?? 5;
            services.AddHangfireServer(options => options.WorkerCount = workerCount);

            // Add metrics configuration
            services.AddMetricsConfiguration(configuration);

            // Register services
            services.AddSingleton<IJobsService, JobsService>();
            
            // Register existing jobs
            services.AddTransient<DataCleanupJob>();
            services.AddTransient<HealthCheckJob>();
            
            // Register new jobs ported from Node.js
            services.AddTransient<InitializeChatMessagesJob>();
            services.AddTransient<ProcessReferrerPayoutsJob>();
            services.AddTransient<ProcessStaleDepositsJob>();
            services.AddTransient<ProcessStaleWithdrawalsJob>();
            services.AddTransient<SendAdvancedGamesMetricsJob>();
            services.AddTransient<SyncBudgetJob>();
            services.AddTransient<UpdateCurrencyRatesJob>();

            services.AddLogging(builder => { builder.AddSerilog(dispose: false); });
        })
        .Build();

    // Get the jobs service and register jobs
    var jobsService = host.Services.GetRequiredService<IJobsService>();
    jobsService.RegisterJobs();

    // Run the host
    await host.RunAsync();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}