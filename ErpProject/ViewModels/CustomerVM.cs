using ErpProject.Models;

namespace ErpProject.ViewModels
{
    public class CustomerVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Order>? orders { get; set; }= new List<Order>();
    }
}
