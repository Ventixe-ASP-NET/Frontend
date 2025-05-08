using Authentication.Entities;
using WebApp.Models;

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
}
