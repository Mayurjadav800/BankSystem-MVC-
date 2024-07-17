using BankSystem_MVC_.Dto;

namespace BankSystem_MVC_.Repository
{
    public interface IAuthenticationRepository
    {
        Task<string> CreateAuthentication(LogginDto logginDto);
    }
}
