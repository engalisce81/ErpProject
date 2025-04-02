using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Purchase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateOnly PurchaseDate { get; set; } 
        public decimal? TotalAmount { get; set; }
        public bool AcceptData { get; set; }

        [ForeignKey(nameof(Supplier))]
        public int? SupplierId { get; set; }
        public Supplier? Supplier { get; set; }

        public ICollection<PurchaseItem>? PurchaseItems { get; set; }=new List<PurchaseItem>(); 
    }
}
