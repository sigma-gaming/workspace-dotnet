using Core.Db.Configuration;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Core.Db;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDb(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<DbContextOptionsBuilder>? optionsAction = null)
    {
        var dbOptions = new CoreDbOptions();
        configuration.Bind("CoreDb", dbOptions);

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