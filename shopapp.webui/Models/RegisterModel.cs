using System.ComponentModel.DataAnnotations;

namespace shopapp.webui.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Prompt = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Prompt = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [Display(Prompt = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Prompt = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Prompt = "Confirm Password")]
        public string RePassword { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Prompt = "Email Address")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Prompt = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}