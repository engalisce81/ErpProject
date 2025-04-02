using System.ComponentModel.DataAnnotations;

namespace ErpProject.ViewModels
{
    public class LoginVM
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password {  get; set; }
        public bool RememberMe {  get; set; }
    }
}
