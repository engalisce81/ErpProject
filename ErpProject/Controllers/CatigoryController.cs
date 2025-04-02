using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ErpProject.Constant;
namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Catigory.View)]
    public class CatigoryController : Controller
    {
        private readonly IRepository<Catigory> _repository;
        private readonly ErpDbContext _context;
        public CatigoryController(IRepository<Catigory> repository, ErpDbContext context)
        {
            _repository = repository;
            _context = context;
        }
        [Authorize(Policy = Permissions.Catigory.View)]
        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys = await GetAllCatigory(searchName);
            return View(entitys);
        }
        [Authorize(Policy = Permissions.Catigory.View)]
        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Catigory.Creat)]

        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            return View();
        }
        [Authorize(Policy = Permissions.Catigory.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Catigory catigory)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(catigory);
                return RedirectToAction(nameof(Index));
            }
            return View(catigory);
        }

        [Authorize(Policy = Permissions.Catigory.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            var catigory = await _repository.GetByIdAsync(id);
            return View(catigory);
        }
        [Authorize(Policy = Permissions.Catigory.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Catigory dep)
        {
            if (ModelState.IsValid)
            {
                var catigory = await _repository.GetByIdAsync(dep.Id);
                catigory.Name = dep.Name;
                _repository.Update(catigory);
                return RedirectToAction(nameof(Index));
            }
            return View(dep);
        }
        [Authorize(Policy = Permissions.Catigory.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var catigory = await _repository.GetByIdAsync(id);
            if (catigory != null)
            {
                try
                {
                    _repository.Delete(catigory);
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
        public async Task<List<Catigory>> GetAllCatigory(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Catigory>().ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Catigory>().Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
    }
}
