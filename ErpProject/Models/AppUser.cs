using Microsoft.AspNetCore.Identity;

namespace ErpProject.Models
{
    public class AppUser:IdentityUser
    {
        
        public string Name {  get; set; }
    }
}
