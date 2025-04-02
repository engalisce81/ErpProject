using System.ComponentModel.DataAnnotations;

namespace ErpProject.ViewModels
{
    public class RegisterVM
    {
        public string Id {  get; set; }=Guid.NewGuid().ToString();
        public string Email { get; set; }
        [Required]
        public string Password {  get; set; }
        [Required]
        [Compare("Password")]
        public string ConfermPassword {  get; set; }
        public string Name {  get; set; }
    }
}
