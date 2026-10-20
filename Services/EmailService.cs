using Leasing.Models;
using Leasing.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Leasing.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendInquiryEmailAsync(EmailViewModel model)
        {
            try
            {
                // Get email settings from appsettings.json
                var smtpServer = _configuration["EmailSettings:SmtpServer"];
                var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var username = _configuration["EmailSettings:Username"];
                var password = _configuration["EmailSettings:Password"];
                var useTLS = bool.Parse(_configuration["EmailSettings:UseTLS"]);

                // Create the email message
                var email = new MimeMessage();

                // Sender
                email.From.Add(new MailboxAddress(senderName, senderEmail));

                // Recipient (the leasing agent)
                email.To.Add(new MailboxAddress(model.AgentName, model.AgentEmail));

                // Reply-to (user who filled out the form)
                email.ReplyTo.Add(new MailboxAddress(model.UserName, model.UserEmail));

                // Subject
                email.Subject = $"New Inquiry: {model.PropertyTitle}";

                // Email body
                var builder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <h2>New Property Inquiry</h2>
                        <p><strong>Property:</strong> {model.PropertyTitle}</p>
                        <p><strong>Inquirer:</strong> {model.UserName}</p>
                        <p><strong>Email:</strong> {model.UserEmail}</p>
                        <p><strong>Phone:</strong> {model.PhoneNumber}</p>
                        <hr />
                        <p><strong>Message:</strong></p>
                        <p>{model.Message}</p>
                        <hr />
                        <p style='color: #666; font-size: 12px;'>
                            This inquiry was sent from the RichPaul Leasing website.
                            Reply directly to this email to contact the inquirer.
                        </p>
                    ",
                    TextBody = $@"
                        New Property Inquiry
                        --------------------
                        Property: {model.PropertyTitle}
                        Inquirer: {model.UserName}
                        Email: {model.UserEmail}
                        Phone: {model.PhoneNumber}

                        Message:
                        {model.Message}

                        This inquiry was sent from the RichPaul Leasing website.
                        Reply directly to this email to contact the inquirer.
                    "
                };

                email.Body = builder.ToMessageBody();

                // Send the email
                using var client = new SmtpClient();

                if (useTLS)
                {
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
                }
                else
                {
                    await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.None);
                }

                await client.AuthenticateAsync(username, password);
                await client.SendAsync(email);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully for property: {model.PropertyTitle}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email for property: {model.PropertyTitle}");
                return false;
            }
        }
    }
}