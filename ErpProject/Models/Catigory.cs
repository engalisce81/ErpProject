namespace ErpProject.Models
{
    public class Catigory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product>? Products { get; set; }=new List<Product>();
    }
}
