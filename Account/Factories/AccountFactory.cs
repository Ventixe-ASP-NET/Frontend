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
            LastName = "Lastname",
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
            lastName = "Lastname";


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


    public static AppUser? EntityToAppUser(AppUserEntity entity, SaveProfileViewModel profile, string role = "")
    {
        if (entity == null) return null;

        var appUserProfile = new AppUserProfile
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Phone = profile.Phone,
            Email = entity?.Email!
        };

        var appUserAddress = new AppUserAddress
        {
            StreetAddress = profile.Address,
            PostalCode = profile.Postal,
            City = profile.City
        };

        var appUser = new AppUser
        {
            Id = entity?.Id!,
            Role = role,
            AppUserProfile = appUserProfile,
            AppUserAddress = appUserAddress
        };

        return appUser;
    }
}
