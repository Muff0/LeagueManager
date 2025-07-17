using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Shared;
using Shared.Settings;

namespace Mail
{
    public class MailService : ServiceBase
    {
        private readonly IOptions<MailSettings> _settings;

        public MailService(IOptions<MailSettings> settings)
        {
            _settings = settings;

        }


        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your Name", _settings.Value.SmtpUsername));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            message.Body = new TextPart("plain")
            {
                Text = body
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(_settings.Value.SmtpServer, _settings.Value.SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_settings.Value.SmtpUsername, _settings.Value.SmtpPassword);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}