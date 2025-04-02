using Microsoft.AspNetCore.Mvc;
using ErpProject.Models;
using ErpProject.Data;
using ErpProject.Repository.Basic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ErpProject.Constant;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Discount.View)]
    public class DiscountController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<Discount> _repository;
        public DiscountController(ErpDbContext context, IRepository<Discount> repository) 
        {
            _context = context;
            _repository = repository; 
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entites=await _context.discounts.Include(e=>e.Employee).Include(dt=>dt.DiscountType).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.Discount.Creat)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await ViewBages();
            return View();
        }
        [Authorize(Policy = Permissions.Catigory.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(Discount entity)
        {
            await ViewBages();
            await operation(entity);
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public async Task operation(Discount entity)
        {
            DiscountType discountType = await _context.discountTypes.FirstOrDefaultAsync(e => e.Id == entity.DiscountTypeId);
            entity.TotalDiscount = discountType.DiscountValue;

        }
        [Authorize(Policy = Permissions.Discount.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await ViewBages();
            var entity =await _repository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Discount.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(Discount entity)
        {
            await ViewBages();
            await operation(entity);
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Discount.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                _repository.Delete(entity);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task ViewBages()
        {
            var employes=await _context.employees.ToListAsync();
            SelectList selectEmployees=new SelectList(employes,"Id","Name");
            var discountTypes=await _context.discountTypes.ToListAsync();
            SelectList selectDiscountTypes = new SelectList(discountTypes, "Id", "Name");
            ViewBag.Employes = selectEmployees;
            ViewBag.DiscountTypes = selectDiscountTypes;
        }
    }
}
