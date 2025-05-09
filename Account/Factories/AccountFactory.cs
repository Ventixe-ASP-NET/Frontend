using Account.Entities;
using Account.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Account.Factories;

public class AccountFactory
{
    public static AppUserEntity MapSignUp(SignUpFormModel formData)
    {
        var user = new AppUserEntity
        {
            UserName = formData.Email,
            Email = formData.Email,
        };

        return user;
    }


    public static AppUserEntity MapSignUpExternal(ExternalLoginInfo info)
    {
        //string firstName = string.Empty;
        //string lastName = string.Empty;

        //try
        //{
        //    firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;
        //    lastName = info.Principal?.FindFirstValue(ClaimTypes.Surname)!;
        //}
        //catch { }

        string email = info?.Principal?.FindFirstValue(ClaimTypes.Email)!;
        string userName = $"ext_{info?.LoginProvider.ToLower()}_{email}";

        var user = new AppUserEntity
        {
            UserName = userName,
            Email = email,
        };

        return user;
    }
}
