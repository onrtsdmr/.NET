using System.ComponentModel.DataAnnotations;

namespace shopapp.webui.Models
{
    public class LoginModel
    {
        //     [Required]
        //     [Display(Prompt = "User Name")]
        //     public string UserName { get; set; }
        [Required]
        [Display(Prompt = "Email Address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Prompt = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Prompt = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string ReturnUrl { get; set; }
    }
}