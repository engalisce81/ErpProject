using ErpProject.Constant;
using ErpProject.Models;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace ErpProject.Seeds
{
    public  class DefaultUsers
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DefaultUsers(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AdminUserRole()
        {
            var admin = await _userManager.FindByEmailAsync("admin@erp.com");
             var roledb = await _roleManager.FindByNameAsync("Admin");
            if (roledb == null) 
            {
                IdentityRole role = new IdentityRole() { Name = "Admin" };
                roledb=role;
                await _roleManager.CreateAsync(role);
                var allPermision = Permissions.GenerateAllPermissions();

                foreach (var permission in allPermision)
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permissions", permission));
                }
            }
            if (admin == null)
            {
                AppUser adminUser = new AppUser() { Name = "Admin User", UserName = "admin@erp.com", Email = "admin@erp.com", EmailConfirmed = true };
                await _userManager.CreateAsync(adminUser,"0100@aA");
                await _userManager.AddToRoleAsync(adminUser, roledb.Name);
                var allPermision = Permissions.GenerateAllPermissions();
                foreach (var permission in allPermision)
                {
                    await _userManager.AddClaimAsync(adminUser, new Claim("Permissions", permission));
                }
            }
        }

    }
}
