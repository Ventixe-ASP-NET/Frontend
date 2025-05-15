using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpEmailViewModel
{
    [Display(Name = "Email")]
    [Required(ErrorMessage = "Email required")]
    [EmailAddress]
    public string Email { get; set; } = null!;
}
