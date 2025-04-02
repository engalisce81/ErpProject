using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class AboutItem
    {
        public int Id { get; set; }
        public string Description {  get; set; }
        [ForeignKey(nameof(ProductId))]
        public int? ProductId {  set; get; }
        public Product? Product { set; get; }

    }
}
