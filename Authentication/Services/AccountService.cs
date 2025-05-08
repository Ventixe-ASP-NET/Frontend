using Account.Factories;
using Account.Interfaces;
using Account.Models;
using Authentication.Entities;
using Microsoft.AspNetCore.Identity;
using WebApp.Models;

namespace Account.Services;

public class AccountService(UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager) : IAccountService
{
    private readonly UserManager<AppUserEntity> _userManager = userManager;
    private readonly SignInManager<AppUserEntity> _signInManager = signInManager;


    public async Task<AccountResult> SignUpAsync(SignUpFormModel formData)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(formData.Email);
            if (existingUser != null)
                throw new Exception("Email already exists");

            var appUserEntity = AccountFactory.MapSignUp(formData);

            var result = await _userManager.CreateAsync(appUserEntity, formData.Password);

            if (!result.Succeeded)
                return new AccountResult { Succeeded = false, Message = string.Join(", ", result.Errors) };

            var roleResult = await _userManager.AddToRoleAsync(appUserEntity, "Admin");
            return roleResult.Succeeded
                ? new AccountResult { Succeeded = true }
                : new AccountResult { Succeeded = true, Message = "User was added but not assigned a role" };
        }
        catch (Exception ex)
        {
            return new AccountResult { Succeeded = false, Message = ex.Message };
        }
    }


    public async Task<AccountResult> SignInAsync(SignInFormModel formData)
    {
        var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, false, false);
        return result.Succeeded
            ? new AccountResult { Succeeded = true }
            : new AccountResult { Succeeded = false, Message = "Invalid email or password." };
    }


    public async Task<AccountResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AccountResult { Succeeded = true };
    }
}
