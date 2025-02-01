using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Common.Logging;

public static class Logging
{
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder, string appName)
    {
        builder.Host.UseSerilog((_, loggerConfiguration) =>
        {
            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithProperty("App", appName);

            if (builder.Environment.IsProduction())
            {
                loggerConfiguration
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .WriteTo.Async(c => c.Console(new CompactJsonFormatter()));
            }
            else
            {
                loggerConfiguration
                    .MinimumLevel.Debug()
                    .WriteTo.Async(c => c.Console());
            }
        });

        return builder;
    }
}