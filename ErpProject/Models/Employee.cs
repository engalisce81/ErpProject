using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpProject.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage ="Pleas Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Pleas Enter Salary")]
        public decimal BasicSalary { get; set; }
        public decimal? FinalSalary { get; set; }
        public DateOnly? HireDate { get; set; }
        [Required(ErrorMessage = "Pleas Enter Phone")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Pleas Enter Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Pleas Enter Address")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Pleas Enter Comment")]
        public string Comment {  get; set; }
        public bool AcceptData { get; set; }
        [ForeignKey(nameof(Department))]
        public int? DepartmentId {  get; set; }
        public Department? Department { get; set; }
        [ForeignKey(nameof(Image))]
        public int? ImageId { get; set; }
        public Image? Image { get; set; }

        public ICollection<Leave>? Leaves { get; set; }=new List<Leave>();
        public ICollection<Discount>? discounts { get; set; } =new List<Discount>();
        public ICollection<Incentive>? incentives { get; set; } =new List<Incentive>();
        public ICollection<AttendanceAndDeparture>? attendanceAndDepartures { get; set; }=new List<AttendanceAndDeparture>();
    }
}
