namespace WebApp.Models;

public class SignUpViewModel
{
    public SignUpFormModel FormData { get; set; } = new();

    public bool AccountCreated { get; set; } = false;
}
