using Common.Db.Interceptors;
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

        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = dbOptions.Host,
            Port = dbOptions.Port,
            Database = dbOptions.Database,
            Username = dbOptions.Username,
            Password = dbOptions.Password
        }.ToString();

        Action<DbContextOptionsBuilder> action = options =>
        {
            options.UseNpgsql(connectionString, npgsqOptions => { npgsqOptions.EnableRetryOnFailure(); });
            options.AddInterceptors(new ForUpdateInterceptor());
            optionsAction?.Invoke(options);
        };

        services.AddDbContext<CoreDbContext>(action, optionsLifetime: ServiceLifetime.Singleton);
        services.AddPooledDbContextFactory<CoreDbContext>(action);

        return services;
    }
}