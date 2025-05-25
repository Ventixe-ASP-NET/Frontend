using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.ProfileViewModels
{
    public class SaveProfileViewModel
    {
        public Guid? Id { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Postal Code")]
        public string Postal { get; set; }

        [Display(Name = "City")]
        public string City { get; set; }
    }
}