using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MvcDHProject.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CustId {  get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column(TypeName = "varchar")]
        [MaxLength(50)]
        public string? City { get; set; }

        [Column(TypeName ="money")]
        public decimal? Balance { get; set; }
        public bool Status { get; set; }
        
    }
}
