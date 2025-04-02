using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.IncentiveType.View)]
    public class IncentiveTypeController : Controller
    {
        private readonly ErpDbContext _context;
        private readonly IRepository<IncentiveType> _repository;
        public IncentiveTypeController(ErpDbContext context, IRepository<IncentiveType> repository)
        {
            _context = context;
            _repository = repository;
        }


        public async Task<IActionResult> Index()
        {
            var entites = await _context.incentivesTypes.Include(i=>i.incentives).ToListAsync();
            return View(entites);
        }

        [Authorize(Policy = Permissions.IncentiveType.Creat)]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Policy = Permissions.IncentiveType.Creat)]
        [HttpPost]
        public async Task<IActionResult> Create(IncentiveType entity)
        {
            if (ModelState.IsValid)
            {
                await _repository.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }


        [Authorize(Policy = Permissions.IncentiveType.Edit)]
        [HttpGet]
        public async Task<IActionResult> Update(int id)

        {
            var disType = await _repository.GetByIdAsync(id);
            return View(disType);

        }
        [Authorize(Policy = Permissions.IncentiveType.Edit)]
        [HttpPost]
        public async Task<IActionResult> Update(IncentiveType entity)
        {
            if (ModelState.IsValid)
            {
                _repository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(entity);
        }

        [Authorize(Policy = Permissions.IncentiveType.Delet)]
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
