using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class PurchaseItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Quantity { get; set; } 
        public decimal? UnitPrice { get; set; }
        public decimal? UnitPricePurchse{ get; set; }
        public decimal? TotalPrice { get; set; }
        public bool AcceptData { get; set; }

        [ForeignKey(nameof(Purchase))]
        public int? PurchaseId { get; set; }
        public Purchase? Purchase { get; set; }
        
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

    }
}
