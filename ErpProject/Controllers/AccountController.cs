using Microsoft.AspNetCore.Mvc;
using ErpProject.Models;
using Microsoft.AspNetCore.Identity;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authentication;
using ErpProject.Constant;
using System.Security.Claims;
using ErpProject.Seeds;
using Microsoft.AspNetCore.Authorization;

namespace ErpProject.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private DefaultUsers _adminUser;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _adminUser = new DefaultUsers(_userManager, _roleManager);
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(loginVM.Email);
                if (user != null)
                {

                    var result=await _signInManager.CheckPasswordSignInAsync(user,loginVM.Password,loginVM.RememberMe);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user,true);
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            return View(loginVM);
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            await _adminUser.AdminUserRole();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (ModelState.IsValid) 
            {
                AppUser user=new AppUser() { Id=registerVM.Id,UserName=registerVM.Email.Trim(),Email=registerVM.Email.Trim(),Name=registerVM.Name,};
                var result=await _userManager.CreateAsync(user,registerVM.Password);
                if (result.Succeeded)
                {
                    var role = await _userManager.GetRolesAsync(user);
                    if (role.Count == 0) 
                        await _userManager.AddToRoleAsync(user, "user");
                    var allPermision = Permissions.GenerateAllPermissions();
                    foreach (var permission in allPermision)
                    {
                        await _userManager.AddClaimAsync(user, new Claim("Permissions", permission));
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                    foreach (var erorr in result.Errors)
                        ModelState.AddModelError("", erorr.Description);    
            }
            return View(registerVM);
        }

        [HttpGet]
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "" });
        }
    }
}
