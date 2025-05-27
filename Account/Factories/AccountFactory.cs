using Account.Entities;
using Account.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WebApp.Models.ProfileViewModels;

namespace Account.Factories;

public class AccountFactory
{
    public static AppUserEntity MapSignUpExternal(ExternalLoginInfo info)
    {
        string email = info?.Principal?.FindFirstValue(ClaimTypes.Email)!;
        string userName = $"ext_{info?.LoginProvider.ToLower()}_{email}";

        var user = new AppUserEntity
        {
            UserName = userName,
            Email = email,
        };

        return user;
    }


    public static SaveProfileViewModel MapProfile(AppUserEntity appUserEntity)
    {
        string[] splitEmail = appUserEntity!.Email!.Split("@");
        string firstName = splitEmail[0];

        var profile = new SaveProfileViewModel
        {
            Id = Guid.Parse(appUserEntity.Id),
            FirstName = firstName,
            LastName = "Last name",
            Phone = "123",
            Address = "Address",
            Postal = "123",
            City = "City"
        };

        return profile;
    }


    public static SaveProfileViewModel MapProfileExternal(ExternalLoginInfo info, AppUserEntity appUserEntity)
    {
        string firstName = string.Empty;
        string lastName = string.Empty;

        try
        {
            firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName)!;
            lastName = info.Principal?.FindFirstValue(ClaimTypes.Surname)!;
        }
        catch { }

        if (string.IsNullOrEmpty(firstName))
        {
            string[] splitEmail = appUserEntity!.Email!.Split("@");
            firstName = splitEmail[0];
        }

        if (string.IsNullOrEmpty(lastName))
            lastName = "Last name";


        var profile = new SaveProfileViewModel
        {
            Id = Guid.Parse(appUserEntity.Id),
            FirstName = firstName,
            LastName = lastName,
            Phone = "123",
            Address = "Address",
            Postal = "123",
            City = "City"
        };

        return profile;
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
