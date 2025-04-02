namespace ErpProject.Models
{
    public class IncentiveType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Incentive>? incentives { get; set; } = new List<Incentive>();
    }
}
