using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleas Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pleas Enter Phone")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Pleas Enter Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Pleas Enter Address")]
        public string Address { get; set;}
        public bool AcceptData { get; set; }
        public List<Order>? orders { get; set; }=new List<Order>();

        [ForeignKey(nameof(Image))]
        public int? ImageId { get; set; }
        public Image? Image { get; set; }
    }
}
