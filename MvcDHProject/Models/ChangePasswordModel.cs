using System.ComponentModel.DataAnnotations;

namespace MvcDHProject.Models
{
    public class ChangePasswordModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Token { get; set; }

        [Required]
        [Display(Name ="Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Confirm password should match with password.")]
        [Display(Name ="Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
