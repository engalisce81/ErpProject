using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Department.View)]
    public class DepartmentController : Controller
    {
        private readonly IRepository<Department> _repository;
        private readonly ErpDbContext _context;
        public DepartmentController(IRepository<Department> repository, ErpDbContext context)
        {
            _repository = repository;
            _context = context;
        }
        [Authorize(Policy = Permissions.Department.View)]
        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys = await GetAllDepartment(searchName);
            return View(entitys);
        }

        [Authorize(Policy = Permissions.Department.View)]
        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Department.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            return View();
        }

        [Authorize(Policy = Permissions.Department.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Department department)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(department);
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [Authorize(Policy = Permissions.Department.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            var department= await _repository.GetByIdAsync(id);
            return View(department);
        }

        [Authorize(Policy = Permissions.Department.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Department dep)
        {
            if (ModelState.IsValid) 
            {
                var department = await _repository.GetByIdAsync(dep.Id);
                department.Name = dep.Name;
                _repository.Update(department);
                return RedirectToAction(nameof(Index));
            }
            return View(dep);
        }
        [Authorize(Policy = Permissions.Department.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var department = await _repository.GetByIdAsync(id);
            if(department != null)
            {
                try
                {
                    _repository.Delete(department);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex) 
                {
                    return RedirectToAction(nameof(Erorr));
                }
            }
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public IActionResult Erorr()
        {
            return View();
        }
        public async Task<List<Department>> GetAllDepartment(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Department>().ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Department>().Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
    }
}
