using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class AttendanceAndDeparture
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly TimeIn { get; set; }
        public TimeOnly TimeOut {  get; set; }
        public decimal? HourLate { get; set; }
        public decimal? DiscountValue { get; set; }
        [ForeignKey(nameof(Employee))]
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }

    }
}
