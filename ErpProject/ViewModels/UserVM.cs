using System.ComponentModel.DataAnnotations;

namespace ErpProject.ViewModels
{
    public class UserVM
    {
        public string Id { get; set; }= Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare(nameof(Password))]
        public string ConfiermPassword {  get; set; }
        public List<RoleVM>? RoleVMs { get; set; }=new List<RoleVM>();

    }
}
