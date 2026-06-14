using MailKit.Net.Smtp;
using MimeKit;

namespace Conqueco.Services.Mail
{
    public class EmailService
    {
        public async Task SendAsync(string to, string subject, string html)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress("Conqueco", "noreply@conqueco.com"));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = html
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("localhost", 1025, false);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}