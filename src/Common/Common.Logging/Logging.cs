using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;

namespace Common.Logging;

public static class Logging
{
    public static void AddLogging(this WebApplicationBuilder builder, string appName)
    {
        builder.Host.UseSerilog((_, loggerConfiguration) =>
        {
            loggerConfiguration.Enrich.FromLogContext();
            loggerConfiguration.Enrich.WithProperty("App", appName);

            if (builder.Environment.IsProduction())
            {
                loggerConfiguration.WriteTo.Console(new CompactJsonFormatter());
            }
            else
            {
                loggerConfiguration.WriteTo.Console();
            }
        });
    }
}