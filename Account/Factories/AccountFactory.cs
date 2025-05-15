using Account.Entities;
using Account.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Account.Factories;

public class AccountFactory
{
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


    public static AppUser? EntityToAppUser(AppUserEntity entity, string role = "")
    {
        if (entity == null) return null;

        var profile = new AppUserProfile
        {
            FirstName = "",
            LastName = "",
            Phone = "",
            Email = entity?.Email!
        };

        var address = new AppUserAddress
        {
            StreetAddress = "",
            PostalCode = "",
            City = ""
        };

        var appUser = new AppUser
        {
            Id = entity?.Id!,
            Role = role,
            AppUserProfile = profile,
            AppUserAddress = address
        };

        return appUser;
    }
}
