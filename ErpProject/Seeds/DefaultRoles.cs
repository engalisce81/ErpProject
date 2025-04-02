using ErpProject.Constant;
using Microsoft.AspNetCore.Identity;

namespace ErpProject.Seeds
{
    public static class DefaultRoles
    {
        public static async Task seedAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Role.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Basic.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));

        }
    }
}
