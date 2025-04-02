using Microsoft.AspNetCore.Mvc;
using ErpProject.Models;
using ErpProject.Data;
using ErpProject.Repository.Basic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ErpProject.Service;
using Microsoft.AspNetCore.Authorization;
using ErpProject.Constant;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.AttendanceAndDeparture.View)]
    public class AttendanceAndDepartureController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<AttendanceAndDeparture> _repository;
        private readonly DiscountOperations _discountOperations;
        public AttendanceAndDepartureController(ErpDbContext context, IRepository<AttendanceAndDeparture> repository)
        {
            _context = context;
            _repository = repository;
            _discountOperations=new DiscountOperations(context);
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entites = await _context.Set<AttendanceAndDeparture>().Include(e=>e.Employee).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.Catigory.Creat)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await ViewBages();
            return View();
        }
        [Authorize(Policy = Permissions.Catigory.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(AttendanceAndDeparture entity)
        {
            await ViewBages();
            _discountOperations.Opearation(entity);
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Catigory.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await ViewBages();
            var entity = await _repository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Catigory.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(AttendanceAndDeparture entity)
        {
            await ViewBages();
            _discountOperations.Opearation(entity);
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Catigory.Delet)]
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
            var employes = await _context.employees.ToListAsync();
            SelectList selectEmployees = new SelectList(employes, "Id", "Name");
            ViewBag.Employes = selectEmployees;
            
        }
    }
}
