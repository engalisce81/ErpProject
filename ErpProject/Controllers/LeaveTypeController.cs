using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.LeaveType.View)]
    public class LeaveTypeController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<LeaveType> _repository;
        public LeaveTypeController(ErpDbContext context, IRepository<LeaveType> repository)
        {
            _context = context;
            _repository = repository;
        }


        public async Task<IActionResult> Index()
        {
            var entites = await _context.leaveTypes.Include(l=>l.Leaves).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.LeaveType.Creat)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Policy = Permissions.LeaveType.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(LeaveType entity)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }


        [Authorize(Policy = Permissions.LeaveType.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)

        {
            var disType = await _repository.GetByIdAsync(id);
            return View(disType);

        }
        [Authorize(Policy = Permissions.LeaveType.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(LeaveType entity)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.LeaveType.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity != null)
            {
                try
                {
                    _repository.Delete(entity);
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
    }
}
