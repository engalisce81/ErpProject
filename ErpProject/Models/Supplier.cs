using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Supplier
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Description {  get; set; }
        public string Summary{ get; set; }
        public string Phone { get; set; } 
        public string Address { get; set; }
        public string Comment {  get; set; }
        public bool AcceptData { get; set; }
        public List<Purchase>? Purchase { get; set; } = new List<Purchase>();
        [ForeignKey(nameof(Image))]
        public int? ImageId {  get; set; } 
        public Image? Image { get; set; }


    }
}
