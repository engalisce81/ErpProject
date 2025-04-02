using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Incentive
    {
        public int Id { get; set; }
        public decimal IncentiveValue { get; set; }
        public DateOnly IncentiveDate {  get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
        
        [ForeignKey(nameof(IncentiveTypeId))]
        public int? IncentiveTypeId { get; set; }
        public IncentiveType? IncentiveType { get; set; }
    }
}
