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
    [Authorize(Policy = Permissions.Order.View)]
    public class OrderController : Controller
    {
        
        private readonly IRepository<Order> _orderrepository;
        private readonly ErpDbContext _context;
        public OrderController(IRepository<Order> orderrepository, ErpDbContext context)
        {
            _orderrepository = orderrepository;
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

        [Authorize(Policy = Permissions.Order.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectList();
            return View();
        }
        [Authorize(Policy = Permissions.Order.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Order order)
        {
            
            if (ModelState.IsValid)
            {
                var customer=await _context.customer.FirstOrDefaultAsync(c=>c.Id == order.CustomerId);
                order.AcceptData = true;
                order.Name=customer.Name;
                order.TotalAmount = 0;
                await _orderrepository.AddAsync(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }
        [Authorize(Policy = Permissions.Order.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            await SelectList();
            var order = await _orderrepository.GetByIdAsync(id);
            return View(order);
        }
        [Authorize(Policy = Permissions.Order.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Order ordervm)
        {
            if (ModelState.IsValid)
            {
                var customer = await _context.customer.FirstOrDefaultAsync(c => c.Id == ordervm.CustomerId);
                ordervm.AcceptData = true;
                ordervm.Name = customer.Name;
                Order order = await GetOrderAsync(ordervm);
                _orderrepository.Update(order);
                return RedirectToAction(nameof(Index));
            }
            return View(ordervm);
        }
        [Authorize(Policy = Permissions.Order.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderrepository.GetByIdAsync(id);
            if (order != null)
            {
                try
                {
                    _orderrepository.Delete(order);
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
        public async Task<List<Order>> GetAllOrder(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Order>().Include(c=>c.Customer).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Order>().Include(c=>c.Customer).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }

        public async Task SelectList()
        {
            var customers=await _context.Set<Customer>().ToListAsync();
            SelectList selectListItems = new SelectList(customers,"Id","Name", "Selected value");
            ViewBag.Customeers=selectListItems;
        }

        public async Task<Order> GetOrderAsync(Order ordervm)
        {
            var order = await _orderrepository.GetByIdAsync(ordervm.Id);
            order.Name = ordervm.Name;
            order.OrderDate = ordervm.OrderDate;
            order.OrderTime = ordervm.OrderTime;
            order.TotalAmount = ordervm.TotalAmount;
            order.AcceptData = ordervm.AcceptData;
            order.CustomerId = ordervm.CustomerId;
            return order;
        }
    }
}
