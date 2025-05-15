using Account.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Account.Interfaces
{
    public interface IAccountService
    {
        Task<bool> AlreadyExistsAsync(string email);
        Task<AppUser?> GetByEmailAsync(string email);
        Task<AppUser?> GetByUserIdAsync(string id);
        Task<AppUser?> GetSignedInAppUserAsync(ClaimsPrincipal? user);
        Task<AccountResult> RequestVerificationCodeAsync(string email);
        Task<AccountResult> SignInAsync(SignInFormModel formData);
        Task<AccountResult> SignOutAsync();
        Task<AccountResult> SignUpAsync(string email, string password);
        Task<AccountResult> SignUpExternalAsync(ExternalLoginInfo info);
        Task<AccountResult> VerifyVerificationCodeAsync(string email, string code);
    }
}