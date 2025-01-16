using Games.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Games.Migrations;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
        await db.Database.MigrateAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                var env = hostingContext.HostingEnvironment;
                
                // Base configuration from appsettings.json
                config.SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                
                // Try to get connection string from environment variables first
                var connectionString = BuildConnectionStringFromEnv(configuration);
                
                // If not found, try to get from appsettings
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = BuildConnectionStringFromConfig(configuration);
                }
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Database connection string could not be constructed");
                }
                
                services.AddDbContext<GamesDbContext>(options =>
                    options.UseNpgsql(connectionString));
            });
            
    private static string BuildConnectionStringFromEnv(IConfiguration configuration)
    {
        var host = configuration["GAMES_DB_HOST"];
        var user = configuration["GAMES_DB_USER"];
        var password = configuration["GAMES_DB_PASSWORD"];
        var database = configuration["GAMES_DB_DATABASE"];
        
        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
        {
            return string.Empty;
        }
        
        return $"Host={host};Database={database};Username={user};Password={password}";
    }
    
    private static string BuildConnectionStringFromConfig(IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("Database");
        
        var host = dbConfig["Host"];
        var user = dbConfig["User"];
        var password = dbConfig["Password"];
        var database = dbConfig["Database"];
        var port = dbConfig["Port"] ?? "5432";
        
        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || 
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(database))
        {
            return string.Empty;
        }
        
        return $"Host={host};Port={port};Database={database};Username={user};Password={password}";
    }
} 