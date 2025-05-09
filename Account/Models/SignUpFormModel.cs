using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpFormModel
{
    [Display(Name = "First name")]
    [Required(ErrorMessage = "First name required")]
    public string FirstName { get; set; } = null!;


    [Display(Name = "Last name")]
    [Required(ErrorMessage = "Last name required")]
    public string LastName { get; set; } = null!;


    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email required")]
    [EmailAddress]
    public string Email { get; set; } = null!;


    [Display(Name = "Password")]
    [Required(ErrorMessage = "Password required")]
    [DataType(DataType.Password)]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$", ErrorMessage = "Must enter a strong password")]
    public string Password { get; set; } = null!;


    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Confirm password required")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;
}
