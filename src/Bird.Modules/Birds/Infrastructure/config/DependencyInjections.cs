using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Config
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddBirdsInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar DbContext para Birds
            services.AddDbContext<BirdsDbContext>(options =>
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection") ?? "Data Source=backbird.db"));

            // Registrar repositorio
            services.AddScoped<IBirdRepository, BirdRepository>();

            return services;
        }
    }
}
