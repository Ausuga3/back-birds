using Microsoft.Extensions.DependencyInjection;
using Bird.Modules.Sightings.Application.Commands;
using Bird.Modules.Sightings.Application.Queries;
using Bird.Modules.Sightings.Domain.Repositories;
using Bird.Modules.Sightings.Infrastructure;

namespace Bird.Modules.Sightings;

public static class SightingsModule
{
    public static IServiceCollection AddSightingsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        // Repository
        services.AddSingleton<ISightingRepository>(
            new SightingRepository(connectionString));

        // Command Handlers
        services.AddScoped<CreateSightingHandler>();
        services.AddScoped<UpdateSightingHandler>();
        services.AddScoped<DeleteSightingHandler>();

        // Query Handlers
        services.AddScoped<GetAllSightingsHandler>();
        services.AddScoped<GetSightingByIdHandler>();
        services.AddScoped<GetSightingsByBirdIdHandler>();

        return services;
    }
}
