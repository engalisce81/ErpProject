using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public DateOnly LeaveFrom { get; set; }
        public DateOnly LeaveTo { get; set; }
        public decimal? TotalDiscount {  get; set; }
        public string Description { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public int? EmployeeId {  get; set; }
        public Employee? Employee { get; set; }
        
        [ForeignKey(nameof(LeaveTypeId))]
        public int? LeaveTypeId { get; set; }
        public LeaveType? LeaveType { get;set; }
    }
}
