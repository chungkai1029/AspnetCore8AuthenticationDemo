using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using AspnetCore8AuthenticationDemo.Models;

namespace AspnetCore8AuthenticationDemo.Services
{
    public class EmailSender : IEmailSender
    {
        private EmailUser _emailUser { get; }

        public EmailSender(IOptions<EmailUser> optionsAccessor)
        {
            _emailUser = optionsAccessor.Value;  
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            MimeMessage mineMessage = new MimeMessage();
            mineMessage.From.Add(MailboxAddress.Parse(_emailUser.Username));
            mineMessage.To.Add(MailboxAddress.Parse(toEmail));
            mineMessage.Subject = subject;

            mineMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp-mail.outlook.com", 587, SecureSocketOptions.StartTls);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(_emailUser.Username, _emailUser.Password);

                await client.SendAsync(mineMessage);
                client.Disconnect(true);
            }
        }
    }
}