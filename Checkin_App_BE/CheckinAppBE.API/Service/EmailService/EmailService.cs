using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Service.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");

            var host = smtpSettings["Host"] ?? throw new InvalidOperationException("SMTP Host not configured.");
            var senderName = smtpSettings["SenderName"] ?? throw new InvalidOperationException("SMTP SenderName not configured.");
            var senderEmail = smtpSettings["SenderEmail"] ?? throw new InvalidOperationException("SMTP SenderEmail not configured.");
            var username = smtpSettings["Username"] ?? throw new InvalidOperationException("SMTP Username not configured.");
            var password = smtpSettings["Password"] ?? throw new InvalidOperationException("SMTP Password not configured.");

            if (!int.TryParse(smtpSettings["Port"], out int port))
            {
                throw new InvalidOperationException("SMTP Port is not a valid number.");
            }

            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress(senderName, senderEmail));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}