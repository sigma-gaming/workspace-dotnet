using Microsoft.Extensions.DependencyInjection;

namespace Core.Builder;

public class CoreBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
}

public static class ServiceCollectionExtensions
{
    public static CoreBuilder Core(this IServiceCollection services)
    {
        return new CoreBuilder(services);
    }
}