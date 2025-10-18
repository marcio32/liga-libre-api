using FluentValidation;
using LigaLibre.Application.DTOs;
using LigaLibre.Application.Interfaces;
using LigaLibre.Application.Mappings;
using LigaLibre.Application.Services;
using LigaLibre.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace LigaLibre.Application
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            //Mapster
            MappingConfig.RegisterMappings();

            //Services
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IPlayerService, PlayerService>();
            services.AddScoped<IMatchService, MatchService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IRefereeService, RefereeService>();

            //Validators
            services.AddScoped<IValidator<CreatePlayerDto>, CreatePlayerValidator>();
            services.AddScoped<IValidator<CreateClubDto>, CreateClubValidator>();
            services.AddScoped<IValidator<CreateMatchDto>, CreateMatchValidator>();
            services.AddScoped<IValidator<CreateRefereeDto>, CreateRefereeValidator>();
            services.AddScoped<IValidator<UpdateRefereeDto>, UpdateRefereeValidator>();
            services.AddScoped<IValidator<UpdatePlayerDto>, UpdatePlayerValidator>();
            services.AddScoped<IValidator<UpdateClubDto>, UpdateClubValidator>();
            services.AddScoped<IValidator<UpdateMatchDto>, UpdateMatchValidator>();

            return services;
        }
    }
}
