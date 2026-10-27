using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Leasing.Services
{
    public class IdentityEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityEmailSender> _logger;

        public IdentityEmailSender(IConfiguration configuration, ILogger<IdentityEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            _logger.LogInformation($"========== IdentityEmailSender CALLED for {email} | Subject: {subject} ==========");
            try
            {
                var smtpServer = _configuration["EmailSettings:SmtpServer"]!;
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);
                var senderName = _configuration["EmailSettings:SenderName"]!;
                var senderEmail = _configuration["EmailSettings:SenderEmail"]!;
                var username = _configuration["EmailSettings:Username"]!;
                var password = _configuration["EmailSettings:Password"]!;
                var useTLS = bool.Parse(_configuration["EmailSettings:UseTLS"]!);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderEmail));
                message.To.Add(new MailboxAddress(email, email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                if (useTLS)
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                else
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.None);

                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Identity email sent to {email}: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send Identity email to {email}");
                throw;
            }
        }
    }
}