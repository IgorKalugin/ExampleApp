using System.Threading.Tasks;

namespace Example.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string subject, string body, string to, string attachmentPath);
    }
}