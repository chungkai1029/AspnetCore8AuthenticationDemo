using AspnetCore8AuthenticationDemo.Models;

namespace AspnetCore8AuthenticationDemo.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}