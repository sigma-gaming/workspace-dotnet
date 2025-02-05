using Core.Builder;
using Core.Db;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        var env = hostingContext.HostingEnvironment;

        config.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Core().AddCoreDb(hostContext.Configuration, options =>
            options.UseNpgsql(x => x.MigrationsAssembly("Core.Migrations")));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
await db.Database.MigrateAsync();