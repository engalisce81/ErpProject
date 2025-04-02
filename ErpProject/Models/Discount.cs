using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Discount
    {
        public int Id { get; set; }
        public DateOnly DateDiscount { get; set; }
        public decimal TotalDiscount {  get; set; }
        [ForeignKey(nameof(DiscountTypeId))]
        public int? DiscountTypeId {  get; set; }
        public string Description {  get; set; }
        public DiscountType? DiscountType { get; set; }
        [ForeignKey(nameof(EmployeeId))]
        public int? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
