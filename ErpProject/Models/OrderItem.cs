using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? Quantity { get; set; } 
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPric {  get ; set; }
        public bool AcceptData { get; set; }
        public bool State { get; set; }
        
        [ForeignKey(nameof(Order))]
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
   

    }
}
