using Games.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Games.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddGamesInfrastructure(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> optionsAction)
    {
        // Register context
        services.AddDbContext<GamesDbContext>(optionsAction);
        
        // Register repositories (to be added later)
        // services.AddScoped<IGameRepository, GameRepository>();

        return services;
    }
} 