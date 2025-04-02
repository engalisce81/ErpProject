using ErpProject.Constant;
using ErpProject.CreatSteps;
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
    [Authorize(Policy = Permissions.OrderItem.View)]
    public class OrderItemController : Controller
    {
        private static Image _image = new Image();
        private readonly IRepository<OrderItem> _OrderItemRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        private readonly OrderOperations orderOperations;
        public OrderItemController(IRepository<OrderItem> OrderItemRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _OrderItemRepository = OrderItemRepository;
            _ImageRepository = ImageRepository;
            _context = context;
            orderOperations = new OrderOperations(_context);
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

        [Authorize(Policy = Permissions.OrderItem.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectListOrder();
            await SelectListProduct();
            return View();
        }
        [Authorize(Policy = Permissions.OrderItem.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(OrderItem orderItem)
        {
            await SelectListOrder();
            await SelectListProduct();
            if (ModelState.IsValid)
            {
                orderItem.AcceptData = true;
                await orderOperations.AddOperation(orderItem);
                return RedirectToAction(nameof(Index));
            }
            return View(orderItem);
        }

        

        //get image from database and convert from binary to file
        public async Task<IActionResult> GetImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image.Data, image.ContentType);
        }
        [Authorize(Policy = Permissions.OrderItem.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
           
            OrderItem entity = await _OrderItemRepository.GetByIdAsync(id);
            if (entity != null)
            {
                entity.State = true;
                await orderOperations.UpdateOperation(entity);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                return RedirectToAction(nameof(Index));

            }
        }




        [Authorize(Policy = Permissions.OrderItem.Delet)]

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            OrderItem entity = await _OrderItemRepository.GetByIdAsync(id);
            if (entity != null)
            {
                await UpdateProductAndOrder(entity);
                _OrderItemRepository.Delete(entity);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task UpdateProductAndOrder(OrderItem entity)
        {
            if (entity.State)
            {
                Order order = await _context.Set<Order>().FirstOrDefaultAsync(o => o.Id == entity.OrderId);
                Product product = await _context.Set<Product>().FirstOrDefaultAsync(p => p.Id == entity.ProductId);
                order.TotalAmount -= entity.TotalPric;
                product.StockQuantity += entity.Quantity;
                _context.Set<Order>().Update(order);
                _context.Set<Product>().Update(product);
                _context.SaveChanges();
            }
        }
        public async Task SelectListOrder()
        {
            var orders = await _context.Set<Order>().ToListAsync();
            SelectList selectLists = new SelectList(orders, "Id", "Name", "Selected value");
            ViewBag.Orders = selectLists;
        }

        public async Task SelectListProduct()
        {
            var products = await _context.Set<Product>().ToListAsync();
            SelectList selectLists = new SelectList(products, "Id", "Name", "Selected value");
            ViewBag.Products = selectLists;
        }
        

        public async Task<OrderItem> GetOrderItem(OrderItem order)
        {
            OrderItem orderitem = await _context.Set<OrderItem>().Include(o=>o.Order).Include(p=>p.Product).FirstOrDefaultAsync(e => e.Id == order.Id);
            orderitem.Quantity = order.Quantity;
            orderitem.UnitPrice = order.UnitPrice;
            orderitem.AcceptData = order.AcceptData;
            orderitem.OrderId = order.OrderId;
            orderitem.ProductId = order.ProductId;
            return orderitem;
        }
        public async Task<List<OrderItem>> GetAllOrderItems(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<OrderItem>().Include(p=>p.Product).ThenInclude(i => i.Images).Include(o => o.Order).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<OrderItem>().Include(p => p.Product).ThenInclude(i => i.Images).Include(o=> o.Order).Where(e => e.Order.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
      
    }
}
