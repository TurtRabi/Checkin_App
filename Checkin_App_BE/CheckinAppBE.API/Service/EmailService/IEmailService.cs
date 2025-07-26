using System.Threading.Tasks;

namespace Service.EmailService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);
    }
}
