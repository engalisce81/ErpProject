using ErpProject.Constant;
using ErpProject.Models;
using ErpProject.Seeds;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ErpProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly DefaultUsers _adminUser;

        public RoleController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _adminUser = new DefaultUsers(_userManager,_roleManager);
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(role=>new RoleVM() { Id=role.Id,Name=role.Name}).ToListAsync();
            await _adminUser.AdminUserRole();
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
               var role=await _roleManager.FindByNameAsync(roleVM.Name);
                if(role == null)
                {
                    role=new IdentityRole() { Id=roleVM.Id,Name=roleVM.Name,NormalizedName=roleVM.Name};
                    var result=await _roleManager.CreateAsync(role);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    else
                        foreach (var erorr in result.Errors)
                            ModelState.AddModelError(string.Empty, erorr.Description);
                }
            }
            return View(roleVM);
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var role=await _roleManager.FindByIdAsync(id);
            RoleVM roleVM = new RoleVM();
            if (role!=null)
                 roleVM = new RoleVM() { Id=role.Id,Name=role.Name};

            return View(roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleVM roleVM)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(roleVM.Id);
                if (role != null)
                {
                    role.Name = roleVM.Name;
                    var result=await _roleManager.UpdateAsync(role);
                    if(result.Succeeded) return RedirectToAction(nameof(Index));
                    else foreach(var erorr in result.Errors) ModelState.AddModelError("",erorr.Description);
                }
            }
            return View(roleVM);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var role=await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                var result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded) return RedirectToAction(nameof(Index));

            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ManagePermision(string Id)
        {
            var role = await _roleManager.FindByIdAsync(Id);
            if (role == null)
                return NotFound();
            var roleClaims = await _roleManager.GetClaimsAsync(role);
            var roleClaimsn = roleClaims.Select(c => c.Value).ToList();
            var allPermissions = Permissions.GenerateAllPermissions().Select(permision => new CheckBoxVM { DisplayValue = permision }).ToList();

            foreach (var permision in allPermissions) if (roleClaimsn.Any(role => role == permision.DisplayValue)) permision.IsSelected = true;

            var permisionVM = new PermisionVM { RoleName = role.Name, RoleId = Id, checkBoxVMs = allPermissions };
            return View(permisionVM);
        }

        [HttpPost]
        public async Task<IActionResult> ManagePermision(PermisionVM permisionVM)
        {
            var role = await _roleManager.FindByIdAsync(permisionVM.RoleId);
            if (role == null)
                return NotFound();
            var allClaims = await _roleManager.GetClaimsAsync(role);
            var users=await _userManager.GetUsersInRoleAsync(role.Name);
            foreach (var claim in allClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
                foreach(var user in users)
                    await _userManager.RemoveClaimAsync(user, claim);
            }
            foreach (var claim in permisionVM.checkBoxVMs)
            {
                if (claim.IsSelected == true)
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.DisplayValue));
                    foreach (var user in users)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("Permission", claim.DisplayValue));
                    }
                }
            }
            var userdb = await _userManager.FindByEmailAsync(User.Identity.Name.Trim());
            await _signInManager.RefreshSignInAsync(userdb);
            return RedirectToAction(nameof(Index));

        }


    }
}
