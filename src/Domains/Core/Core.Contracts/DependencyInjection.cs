using Core.Contracts.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Contracts;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreMappers(
        this IServiceCollection services)
    {
        services.AddSingleton<BalanceMapper>();
        services.AddSingleton<UserMapper>();
        services.AddSingleton<AccountMapper>();

        return services;
    }
}