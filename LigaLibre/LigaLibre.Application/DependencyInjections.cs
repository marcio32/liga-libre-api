using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Services;
using LigaLibre.Application.Validators;
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
            services.AddScoped<IValidator<CreatePlayerDto>, CreatePlayerValidator>();
            services.AddScoped<IValidator<CreateClubDto>, CreateClubValidator>();
            return services;
        }
    }
}
