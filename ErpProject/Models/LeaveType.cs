namespace ErpProject.Models
{
    public class LeaveType
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal DiscountValue { get; set; }
        public ICollection<Leave>? Leaves { get; set; }=new List<Leave>();
    }
}
