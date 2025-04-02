using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ErpProject.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly HomeOperation _homeOperation;
        public HomeController(ErpDbContext context)
        {
            _context = context;
            _homeOperation=new HomeOperation(_context);
        }
        public async Task<IActionResult> Index()
        {
            await ViewBageOperation();
            await _homeOperation.YearOperation();
            
            var year =await _context.totalYears.Where(y=>y.Year==DateTime.Now.Year).FirstOrDefaultAsync();
            if (year == null)
                year = new TotalYear();
            return View(year);
        }  

        public async Task ViewBageOperation()
        {
            var orderItems =await  _context.orderItems.Include(p=>p.Product).Include(o=>o.Order).ToListAsync();
            var newTransaction=orderItems.TakeLast(3).ToList();
            var customers = await _context.Set<Customer>().ToListAsync();
            var newCustomer = customers.TakeLast(3).ToList();
            var years = await _context.totalYears.ToListAsync();
            var sortByYear = years.OrderBy(o => o.Year).ToList();
            var totoalsales = sortByYear.Select(t => t.TotalSales).ToList();
            var totoalYears = sortByYear.Select(y => y.Year).ToList();
            ViewBag.TotoalYears = totoalYears;
            ViewBag.Totoalsales = totoalsales;
                ViewBag.NewTransaction=newTransaction;
                ViewBag.NewCustomer=newCustomer;
        }
    }
}
