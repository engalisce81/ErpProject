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
    [Authorize(Roles ="Admin")]
    public class SaleController : Controller
    {
        private readonly IRepository<Order> _salerepository;
        private readonly ErpDbContext _context;
        public SaleController(IRepository<Order> Salerepository, ErpDbContext context)
        {
            _salerepository = Salerepository;
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys = await GetAllOrder(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }
        public async Task<List<Order>> GetAllOrder(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Order>().Include(c => c.Customer).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Order>().Include(c => c.Customer).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }

    }
}
