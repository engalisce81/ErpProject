using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Incentive.View)]
    public class IncentiveController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<Incentive> _repository;
        public IncentiveController(ErpDbContext context, IRepository<Incentive> repository)
        {
            _context = context;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entites = await _context.incentives.Include(e=>e.Employee).Include(i=>i.IncentiveType).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.Incentive.Creat)]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await ViewBages();
            return View();
        }
        [Authorize(Policy = Permissions.Incentive.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(Incentive entity)
        {
            await ViewBages();
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Incentive.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            await ViewBages();
            var entity = await _repository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Incentive.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(Incentive entity)
        {
            await ViewBages();
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.Incentive.Delet)]
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
            var incentiveTypes = await _context.incentivesTypes.ToListAsync();
            SelectList selectIncentiveTypes = new SelectList(incentiveTypes, "Id", "Name");
            ViewBag.Employes = selectEmployees;
            ViewBag.IncentiveTypes = selectIncentiveTypes;
        }
    }
}
