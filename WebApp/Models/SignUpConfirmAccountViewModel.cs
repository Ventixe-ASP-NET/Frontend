using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class SignUpConfirmAccountViewModel
{
    [Required]
    public string Email { get; set; } = null!;

    [Display(Name = "Code")]
    [Required(ErrorMessage = "Code required")]
    public string Code { get; set; } = null!;
}


