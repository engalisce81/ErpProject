using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; } 
        public decimal Price { get; set; } 
        public int? StockQuantity { get; set; }
        public bool AcceptData { get; set; }
        public ICollection<PurchaseItem>? purchaseItems { get; set; }=new List<PurchaseItem>();
        public ICollection<Image>? Images { get; set;} = new List<Image>();
        public ICollection<AboutItem>? AboutItems { get;set; } = new List<AboutItem>();
        [ForeignKey(nameof (Catigory))]
        public int? CatigoryId { get; set; }
        public Catigory? Catigory { get; set; }

    }
}
