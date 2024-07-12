using BankSystem_MVC_.Models;

namespace BankSystem_MVC_.Repository
{
    public interface IEmailRepository
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
