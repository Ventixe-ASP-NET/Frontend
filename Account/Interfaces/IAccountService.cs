using Account.Models;
using Microsoft.AspNetCore.Identity;

namespace Account.Interfaces
{
    public interface IAccountService
    {
        Task<AccountResult> SignInAsync(SignInFormModel formData);
        Task<AccountResult> SignOutAsync();
        Task<AccountResult> SignUpAsync(SignUpFormModel formData);
        Task<AccountResult> SignUpExternalAsync(ExternalLoginInfo info);
    }
}