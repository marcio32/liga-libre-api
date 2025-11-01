using LigaLibre.EmailService.Interfaces;
using LigaLibre.EmailService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LigaLibre.EmailService;

public static class DependencyInjections
{
    public static IServiceCollection AddEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddScoped<IEmailService, Services.EmailService>();
        return services;
    }
}

