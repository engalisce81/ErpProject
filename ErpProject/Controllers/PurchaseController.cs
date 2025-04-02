using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.Service;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Purchase.View)]
    public class PurchaseController : Controller
    {
        private readonly IRepository<Purchase> _repository;
        private readonly ErpDbContext _context;
        public PurchaseController(IRepository<Purchase> repository, ErpDbContext context)
        {
            _repository = repository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys = await GetAllPurchase(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Purchase.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectedList();
            return View();
        }
        [Authorize(Policy = Permissions.Purchase.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Purchase purchase)
        {
            await SelectedList();
            if (ModelState.IsValid)
            {
                purchase.TotalAmount=0;
                await _repository.AddAsync(purchase);
                return RedirectToAction(nameof(Index));
            }
            return View(purchase);
        }

        [Authorize(Policy = Permissions.Purchase.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            await SelectedList();
            var purchase = await _repository.GetByIdAsync(id);
            return View(purchase);
        }
        [Authorize(Policy = Permissions.Purchase.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Purchase purchase)
        {
            await SelectedList();
            if (ModelState.IsValid)
            {
                _repository.Update(purchase);
                return RedirectToAction(nameof(Index));
            }
            return View(purchase);
        }
        [Authorize(Policy = Permissions.Purchase.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var purchase = await _repository.GetByIdAsync(id);
            if (purchase != null)
            {
                _repository.Delete(purchase);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));

        }
        public async Task<List<Purchase>> GetAllPurchase(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Purchase>().Include(s=>s.Supplier).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Purchase>().Include(s=>s.Supplier).Where(e => e.Supplier.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }

        public async Task SelectedList()
        {
            List<Supplier> suppliers=await _context.Set<Supplier>().ToListAsync();
            SelectList suppliersItems = new SelectList(suppliers, "Id", "Name");
            ViewBag.Suppliers = suppliersItems;
        }
    }
}
