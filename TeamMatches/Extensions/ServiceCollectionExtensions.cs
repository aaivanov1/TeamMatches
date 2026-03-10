using Microsoft.EntityFrameworkCore;
using TeamMatches.Application.Interfaces;
using TeamMatches.Application.Mappings;
using TeamMatches.Application.Services;
using TeamMatches.Domain.Interfaces;
using TeamMatches.Infrastructure.Persistance;
using TeamMatches.Infrastructure.Repositories;

namespace TeamMatches.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(
        this IServiceCollection services,
        IConfiguration configuration)
        {
            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IGameRepository, GameRepository>();

            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<ILeaderboardService, LeaderboardService>();


            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            return services;
        }
    }
}
