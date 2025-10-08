using Amazon.SQS;
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

            //Redis Cache 
            var redisConnection = configuration.GetConnectionString("Redis") ?? "localhost:6349";
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "LigaLibreAPI";
            });

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
            services.AddScoped<IMatchRepository, MatchRepository>();

            //AWS SQS
            services.AddSingleton<IAmazonSQS>(provider =>
            {
                var config = new AmazonSQSConfig
                {
                    ServiceURL = "http://localhost:4566",
                    UseHttp = true
                };
                return new AmazonSQSClient("test", "test", config);
            });

            //Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISqsService, SqsService>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            return services;
        }
    }
}
