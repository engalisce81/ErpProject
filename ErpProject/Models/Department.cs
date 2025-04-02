using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ErpProject.Models
{
    public class Department
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Pleas Enter Name")]
        public string Name { get; set; }
        public bool AcceptData { get; set; }
        public  List<Employee>? employees { get; set; }=new List<Employee>();
    }
}
