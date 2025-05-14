using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpPasswordViewModel
{
    [Required]
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


