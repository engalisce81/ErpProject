using ErpProject.Constant;
using ErpProject.Models;
using ErpProject.Seeds;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ErpProject.Controllers
{
    [Authorize(Roles = "Admin")]

    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DefaultUsers _adminUser;
        public UserController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) 
        {
            _userManager=userManager;
            _roleManager=roleManager;
            _adminUser = new DefaultUsers(_userManager,_roleManager);
        }
        public async Task<IActionResult> Index()
        {
          

            var userVM = new List<UserVM>();
           var users= await _userManager.Users.ToListAsync();
            var roles = await _roleManager.Roles.ToListAsync();
            foreach(var user in   users)
            {
                var roleVM = new List<RoleVM>();

                var rolesdb =await _userManager.GetRolesAsync(user);
                foreach (var role in rolesdb)
                    roleVM.Add(new RoleVM() {  Name = role });
                userVM.Add(new UserVM() { Id=user.Id,Name=user.Name,Email=user.Email,RoleVMs=roleVM});
            }
            await _adminUser.AdminUserRole();
            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var roleVMS=await _roleManager.Roles.Select(role => new RoleVM() { Id=role.Id,Name=role.Name,IsInRole=false}).ToListAsync();
            UserVM userVM=new UserVM() ;
            userVM.RoleVMs=roleVMS;
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser() {  Name = userVM.Name,UserName=userVM.Email, Email = userVM.Email,EmailConfirmed=true };
                var result=await _userManager.CreateAsync(user, userVM.Password);
                if (result.Succeeded)
                {
                    if (userVM.RoleVMs.Count > 0)
                    {
                        List<string> roles = new List<string>();
                        foreach (var role in userVM.RoleVMs) if (role.IsInRole == true) roles.Add(role.Name);
                        await _userManager.AddToRolesAsync(user, roles);
                    }
                    var allPermision = Permissions.GenerateAllPermissions();
                    foreach (var permission in allPermision)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("Permissions", permission));
                    }
                    return RedirectToAction(nameof(Index));
                }
                else
                    foreach (var erorr in result.Errors)
                        ModelState.AddModelError(string.Empty, erorr.Description);
            }
            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string Id)
        {
            UserUpdateVM userVM;
            List<RoleVM> roleVMs=new List<RoleVM>();
            var user= await _userManager.FindByIdAsync(Id);
            var userRoles=await _userManager.GetRolesAsync(user);
            var roles=await _roleManager.Roles.ToListAsync();

            if (user != null)
            {
                for (int j=0;j< roles.Count; j++)
                {
                    roleVMs.Add(new RoleVM() { Name = roles[j].Name, IsInRole = false });
                    for (int i = 0; i < userRoles.Count; i++)
                    {
                        if (roles[j].Name == userRoles[i])
                        {
                            roleVMs[j].IsInRole = true;
                        }

                    }

                }
                userVM=new UserUpdateVM() { Id=user.Id,Name=user.Name,Email=user.Email,RoleVMs=roleVMs};
                return View(userVM);
            }
            userVM = new UserUpdateVM();
            return View(userVM);
        }

        [HttpPost]
        public async Task<IActionResult>  Update(UserUpdateVM userVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user=await _userManager.FindByIdAsync(userVM.Id);
                if (user != null) 
                {
                    user.Email = userVM.Email;
                    user.Name = userVM.Name;
                    var roles= await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, roles);
                    List<string> rolesVM=new List<string>();
                    foreach(var role in userVM.RoleVMs) if(role.IsInRole==true) rolesVM.Add(role.Name);
                    await _userManager.AddToRolesAsync(user,rolesVM);
                    var result=await _userManager.ChangePasswordAsync(user,userVM.CurrentPassword,userVM.NewPassword);
                    if (result.Succeeded) return RedirectToAction(nameof(Index));
                    else foreach (var erorr in result.Errors) ModelState.AddModelError(string.Empty, erorr.Description);

                }

            }
            return View(userVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user =await  _userManager.FindByIdAsync(id);
            if (user != null)
            {
                var result=await _userManager.DeleteAsync(user);
                if (result.Succeeded) return RedirectToAction(nameof(Index));
                
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
