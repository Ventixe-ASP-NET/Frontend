using System.ComponentModel.DataAnnotations;

namespace Account.Models;

public class SignInFormModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email required")]
    [EmailAddress]
    public string Email { get; set; } = null!;


    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;
}
