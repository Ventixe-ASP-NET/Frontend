using Account.Models;
using WebApp.Models;

namespace Account.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResult> SignInAsync(SignInFormModel formData);
        Task<AccountResult> SignOutAsync();
        Task<AccountResult> SignUpAsync(SignUpFormModel formData);
    }
}