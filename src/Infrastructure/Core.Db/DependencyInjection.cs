using Common.Db.Interceptors;
using Core.Builder;
using Core.Db.Configuration;
using Core.Db.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Core.Db;

public static class DependencyInjection
{
    public static CoreBuilder AddCoreDb(
        this CoreBuilder builder,
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
            Password = dbOptions.Password,
            Pooling = true,
            MinPoolSize = 10,
            MaxPoolSize = dbOptions.MaxConnections,
            NoResetOnClose = true,
            Enlist = false,
            SslMode = SslMode.Disable,
            KeepAlive = 10,
        }.ToString();

        Action<DbContextOptionsBuilder> action = options =>
        {
            options.UseNpgsql(connectionString, npgsqOptions =>
            {
                npgsqOptions.EnableRetryOnFailure();
                npgsqOptions.SetPostgresVersion(17, 0);
                npgsqOptions.ConfigureDataSource(datasource => { datasource.EnableDynamicJson(); });
            });
            options.AddInterceptors(new ForUpdateInterceptor());
            optionsAction?.Invoke(options);
        };

        builder.Services.AddDbContextPool<CoreDbContext>(action);
        builder.Services.AddPooledDbContextFactory<CoreDbContext>(action);

        return builder;
    }
}