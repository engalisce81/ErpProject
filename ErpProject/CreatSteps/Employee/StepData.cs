using System.ComponentModel.DataAnnotations;

namespace ErpProject.CreatSteps.Employee

{
    public class StepData
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please Enter Name ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Please Enter Email ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Description")]
        public string Description { get; set; }
        public string? Summary { get; set; }
        [Required(ErrorMessage = "Please Enter Phone ")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Please Enter Addres")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please Enter Comment ")]
        public string Comment { get; set; }
    }
}
