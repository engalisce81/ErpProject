using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ErpProject.Models
{
    public class Image
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public byte[] Data { get; set; }       
        public string ContentType { get; set; } 
        public string FileName { get; set; }
        public bool Accepted { get; set; }
        public Supplier? Supplier { get; set; }
        public Employee? Employee { get; set; }
        public Customer? Customer { get; set; }
        [ForeignKey(nameof(Product))]
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public Purchase? Purchase { get; set; }
        public SalesInvoice? SalesInvoice { get; set; }
    }
}
