using System.ComponentModel.DataAnnotations;

namespace ErpProject.ViewModels
{
    public class RoleVM
    {
        public string Id {  get; set; }= Guid.NewGuid().ToString();
        [Required]
        public string Name { get; set; }
        public bool IsInRole {  get; set; }
    }
}
