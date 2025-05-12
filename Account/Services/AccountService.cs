using Account.Entities;
using Account.Factories;
using Account.Interfaces;
using Account.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace Account.Services;

public class AccountService(UserManager<AppUserEntity> userManager, SignInManager<AppUserEntity> signInManager, HttpClient httpClient, IConfiguration configuration) : IAccountService
{
    private readonly UserManager<AppUserEntity> _userManager = userManager;
    private readonly SignInManager<AppUserEntity> _signInManager = signInManager;
    private readonly HttpClient _http = httpClient;
    private readonly IConfiguration _configuration = configuration;


    public async Task<bool> AlreadyExistsAsync(string email)
    {
        try
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result != null)
                return true;

            return false;
        }
        catch
        {
            return false;
        }
    }


    private async Task<HttpResponseMessage> VerificationServicePostAsync(string endPoint, object data)
    {
        var baseUrl = "https://ventixe-vsp-facafmejhdcaezdz.swedencentral-01.azurewebsites.net/api/verification/";
        var request = new HttpRequestMessage(HttpMethod.Post, baseUrl + endPoint)
        {
            Content = JsonContent.Create(data)
        };

        request.Headers.Add("x-api-key", _configuration["SecretKeys:VerificationService"]);

        return await _http.SendAsync(request);
    }


    public async Task<AccountResult> RequestVerificationCodeAsync(string email)
    {
        try
        {
            var result = await VerificationServicePostAsync("send", new { email });
            if (result.IsSuccessStatusCode)
                return new AccountResult { Succeeded = true };

            return new AccountResult { Succeeded = false };
        }
        catch (Exception ex)
        {
            return new AccountResult { Succeeded = false, Message = ex.Message };
        }
    }


    public async Task<AccountResult> VerifyVerificationCodeAsync(string email, string code)
    {
        try
        {
            var result = await VerificationServicePostAsync("verify", new { email, code });
            if (result.IsSuccessStatusCode)
                return new AccountResult { Succeeded = true };

            return new AccountResult { Succeeded = false };
        }
        catch (Exception ex)
        {
            return new AccountResult { Succeeded = false, Message = ex.Message };
        }
    }


    public async Task<AccountResult> SignUpAsync(string email, string password)
    {
        try
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
                throw new Exception("Email already exists");

            var appUserEntity = new AppUserEntity
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
            };

            var result = await _userManager.CreateAsync(appUserEntity, password);

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


    public async Task<AccountResult> SignUpExternalAsync(ExternalLoginInfo info)
    {
        try
        {
            var appUserEntity = AccountFactory.MapSignUpExternal(info);

            var existingUser = await _userManager.FindByEmailAsync(appUserEntity.Email!);
            if (existingUser != null)
                throw new Exception("Email already exists");

            var result = await _userManager.CreateAsync(appUserEntity);

            if (!result.Succeeded)
                return new AccountResult { Succeeded = false, Message = string.Join(", ", result.Errors) };

            if (result.Succeeded)
            {
                await _userManager.AddLoginAsync(appUserEntity, info);
                //await _signInManager.SignInAsync(appUserEntity, isPersistent: false);
            }

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
