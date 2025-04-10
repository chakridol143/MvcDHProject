using System.ComponentModel.DataAnnotations;

namespace MvcDHProject.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name ="User Name")]
        public string Name { get; set; }
    }
}
