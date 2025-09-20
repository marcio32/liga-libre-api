using LigaLibre.Application.Interfaces;
using LigaLibre.Domain.Entities;
using LigaLibre.Domain.Interfaces;
using LigaLibre.Infrastructure.Data;
using LigaLibre.Infrastructure.Repositories;
using LigaLibre.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LigaLibre.Infrastructure
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //Database
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            //Identity
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            //Repositories
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();

            //Services
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
