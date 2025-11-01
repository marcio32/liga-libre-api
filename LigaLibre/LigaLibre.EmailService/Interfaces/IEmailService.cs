using LigaLibre.EmailService.Models;

namespace LigaLibre.EmailService.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}