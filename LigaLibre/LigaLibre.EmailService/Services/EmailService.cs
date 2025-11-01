using LigaLibre.EmailService.Models;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using LigaLibre.EmailService.Interfaces;

namespace LigaLibre.EmailService.Services;

public class EmailService(IOptions<EmailSettings> settings) : IEmailService
{
    public async Task SendEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(settings.Value.SenderEmail));
        email.To.Add(MailboxAddress.Parse(message.To));
        email.Subject = message.Subject;

        var builder = new BodyBuilder();
        if (message.IsHtml)
            builder.HtmlBody = message.Body;
        else
            builder.TextBody = message.Body;

        email.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(settings.Value.SmtpServer, settings.Value.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(settings.Value.SenderEmail, settings.Value.Password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

    }
}

