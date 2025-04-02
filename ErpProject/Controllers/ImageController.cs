using ErpProject.Constant;
using ErpProject.Data;
using ErpProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Controllers
{
    [Authorize(Roles="Admin")]
    public class ImageController : Controller
    {
        private readonly ErpDbContext _context;
        public ImageController(ErpDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var images=await _context.images.ToListAsync();
            return View(images);
        }

        public async Task<IActionResult> GetImage(int id)
        {
            Image image = await _context.images.FirstOrDefaultAsync(e => e.Id == id);
            if (image == null)
            {
                return NotFound();
            }
            return File(image.Data, image.ContentType);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Image entity = await _context.images.FirstOrDefaultAsync(e=>e.Id==id);
            if (entity != null)
            {
                _context.Remove(entity);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
