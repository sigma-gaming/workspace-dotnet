using Core.Database.Configuration;
using Core.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Core.Database;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDatabase(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        var dbOptions = new CoreDbOptions();
        configuration.Bind("CoreDB", dbOptions);

        services.AddSingleton(dbOptions);

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = dbOptions.Host,
            Port = dbOptions.Port,
            Database = dbOptions.Database,
            Username = dbOptions.Username,
            Password = dbOptions.Password
        }.ToString();

        services.AddDbContext<CoreDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
            optionsAction?.Invoke(options);
        });

        return services;
    }
} 