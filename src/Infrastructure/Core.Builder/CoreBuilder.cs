using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Core.Builder;

public class CoreBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;
    
    public IHostEnvironment? Environment { get; private set; }

    public CoreBuilder WithEnvironment(IHostEnvironment environment)
    {
        Environment = environment;
        return this;
    }
}

public static class ServiceCollectionExtensions
{
    public static CoreBuilder Core(this IServiceCollection services)
    {
        return new CoreBuilder(services);
    }
}