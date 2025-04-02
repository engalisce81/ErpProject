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
    public class SalesItemController : Controller
    {
        private static Image _image = new Image();
        private readonly IRepository<OrderItem> _OrderItemRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        public SalesItemController(IRepository<OrderItem> OrderItemRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _OrderItemRepository = OrderItemRepository;
            _ImageRepository = ImageRepository;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {

            var entitys = await GetAllOrderItems(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        public async Task<List<OrderItem>> GetAllOrderItems(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<OrderItem>().Include(p => p.Product).Include(o => o.Order).Where(e=>e.State==true).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<OrderItem>().Include(p => p.Product).Include(o => o.Order).Where(e => e.Order.Name.ToLower().Contains(searcName.ToLower()) && e.State==true).ToListAsync();
                return entites;
            }
        }
        public async Task<IActionResult> GetImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image.Data, image.ContentType);
        }


    }
}
