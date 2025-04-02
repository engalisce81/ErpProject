using System.ComponentModel.DataAnnotations;

namespace ErpProject.ViewModels
{
    public class UserUpdateVM
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfiermPassword { get; set; }
        public List<RoleVM>? RoleVMs { get; set; } = new List<RoleVM>();
    }
}
