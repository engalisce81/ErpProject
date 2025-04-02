using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.DiscountType.View)]
    public class DiscountTypeController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<DiscountType> _repository;
        public DiscountTypeController(ErpDbContext context, IRepository<DiscountType> repository) 
        {
            _context = context;
            _repository = repository;
        }


        public async Task<IActionResult> Index()
        {
            var entites=await _context.discountTypes.Include(d=>d.Discounts).ThenInclude(e=>e.Employee).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.DiscountType.Creat)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Policy = Permissions.DiscountType.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(DiscountType entity)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }


        [Authorize(Policy = Permissions.DiscountType.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)

        {
            var disType=await _repository.GetByIdAsync(id);
            return View(disType);
            
        }
        [Authorize(Policy = Permissions.DiscountType.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(DiscountType entity)
        {
            if (ModelState.IsValid) 
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.DiscountType.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                try
                {
                    _repository.Delete(entity);
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

    }
}
