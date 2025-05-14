using Account.Models;

namespace WebApp.Models;

public class SignInViewModel
{
    public SignInFormModel FormData { get; set; } = new();

    public bool AccountCreated { get; set; } = false;
}
