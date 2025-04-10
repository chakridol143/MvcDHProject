using System.ComponentModel.DataAnnotations;

namespace MvcDHProject.Models
{
    public class UserViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Confirm password should match with Password.")]
        public string ConfirmPassword {  get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [DataType(DataType.PhoneNumber)]
        [RegularExpression("[6-9]\\d{9}",ErrorMessage ="Mobile number is InValid.")]
        public string Mobile { get; set; }
    }
}
