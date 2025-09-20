using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LigaLibre.Application
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Services
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IPlayerService, PlayerService>();
            return services;
        }
    }
}
