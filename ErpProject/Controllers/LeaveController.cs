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
    [Authorize(Policy = Permissions.Leave.View)]
    public class LeaveController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<Leave> _repository;
        public LeaveController(ErpDbContext context, IRepository<Leave> repository)
        {
            _context = context;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entites = await _context.leaves.Include(lt=>lt.LeaveType).Include(e=>e.Employee).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.Leave.Creat)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await ViewBages();
            return View();
        }
        [Authorize(Policy = Permissions.Leave.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(Leave entity)
        {
            await ViewBages();
            await Operation(entity);
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        public async Task Operation(Leave entity)
        {
            LeaveType leaveType=await _context.leaveTypes.FirstOrDefaultAsync(e=>e.Id==entity.LeaveTypeId);
            var months = entity.LeaveTo.Month - entity.LeaveFrom.Month ;
            if (months == 0)
            {
                var days = entity.LeaveTo.Day - entity.LeaveFrom.Day ;
                entity.TotalDiscount = leaveType.DiscountValue * days;
            }
            else
            {
                var days = entity.LeaveTo.Day - entity.LeaveFrom.Day;
                months--;
                days += months * 30;
                entity.TotalDiscount = leaveType.DiscountValue * days;
            }
        }

        [Authorize(Policy = Permissions.Leave.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await ViewBages();
            var entity = await _repository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Leave.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(Leave entity)
        {
            await ViewBages();
            await Operation(entity);
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Leave.Delet)]
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
            var leaveTypes = await _context.leaveTypes.ToListAsync();
            SelectList selectLeaveTypes = new SelectList(leaveTypes, "Id", "Name");
            ViewBag.Employes = selectEmployees;
            ViewBag.LeaveTypes = selectLeaveTypes;
        }
    }
}
 