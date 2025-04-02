using ErpProject.Constant;
using ErpProject.CreatSteps;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.Service;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IO;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Product.View)]
    public class ProductController : Controller
    {
        private static Product _product = new Product();
        private static Image _image = new Image();
        private readonly IRepository<Product> _ProductRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        public ProductController(IRepository<Product> ProductRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _ProductRepository = ProductRepository;
            _ImageRepository = ImageRepository;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys = await GetAllProducts(searchName);
            return View(entitys);
        }

        [HttpGet]
        public async Task<IActionResult> ListProduct()
        {
            var products=await _context.products.Include(a=>a.AboutItems).Include(p => p.purchaseItems).ThenInclude(p => p.Purchase).ThenInclude(s => s.Supplier).Include(i => i.Images).Include(c => c.Catigory).ToListAsync();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Product.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectedList();
            return View();
        }
        [Authorize(Policy = Permissions.Product.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Product pro)
        {
            await SelectedList();
            if (ModelState.IsValid)
            {
                pro.StockQuantity = 0;
                pro.AcceptData = true;
                await _ProductRepository.AddAsync(pro);
                return RedirectToAction(nameof(Index));
            }
            return View(pro);
        }


        [Authorize(Policy = Permissions.Product.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateImage(int id)
        {
            ImageVM imageVM = new ImageVM() { ProductId = id };
            return View(imageVM);
        }
        [Authorize(Policy = Permissions.Product.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateImage(ImageVM imageVM)
        {
            if (ModelState.IsValid)
            {
                await AddImage(imageVM);
                return RedirectToAction(nameof(Index));
            }
            return View(imageVM);
        }

        public async Task AddImage(ImageVM imageVm)
        {
            foreach (var formfile in imageVm.FormFiles )
            {
                using (var memoryStream = new MemoryStream())
                {
                    await formfile.CopyToAsync(memoryStream);
                    Image _image = new Image();
                    _image.Data = memoryStream.ToArray();
                    _image.FileName = formfile.FileName;
                    _image.ContentType = formfile.ContentType;
                    _image.Accepted = true;
                    _image.ProductId = imageVm.ProductId;
                    await  _context.images.AddAsync(_image);
                    await _context.SaveChangesAsync();

                }
            }
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
        [Authorize(Policy = Permissions.Product.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            await SelectedList();
            Product entity = await _ProductRepository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Product.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Product pro)
        {
            await SelectedList();
            Product product = await GetProductAndImage(pro);
            if (ModelState.IsValid)
            {
                _ProductRepository.Update(product);
                return RedirectToAction(nameof(Index));
            }
            return View(pro);
        }

        [Authorize(Policy = Permissions.Product.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            StepImage stepImage = ConvertStream.ConvertFromStreamToFormFile(image);
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Product.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditImage(StepImage stepImage)
        {
            if (ModelState.IsValid)
            {
                Image image = await _ImageRepository.GetByIdAsync(stepImage.Id);
                //await ConvertToImage.AsignImage(stepImage, image);
                using (var memoryStream = new MemoryStream())
                {
                    await stepImage.FormFile.CopyToAsync(memoryStream);
                    image.Data = memoryStream.ToArray();
                    image.FileName = stepImage.FormFile.FileName;
                    image.ContentType = stepImage.FormFile.ContentType;
                    image.Accepted = true;
                }
                _ImageRepository.Update(image);
                return RedirectToAction(nameof(Index));
            }
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Product.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.products.Include(i=>i.Images).Include(a=>a.AboutItems).FirstOrDefaultAsync(p=>p.Id==id);
            if (product != null)
            {
                try
                {
                    foreach (Image image in product.Images)
                    {
                        Image imagedb = await _context.images.FirstOrDefaultAsync(p => p.Id == image.Id);
                        _context.Remove(imagedb);
                    }
                    foreach (AboutItem about in product.AboutItems)
                    {
                        AboutItem aboutdb = await _context.aboutItems.FirstOrDefaultAsync(p => p.Id == about.Id);
                        _context.Remove(aboutdb);
                    }
                    await _context.SaveChangesAsync();
                    _ProductRepository.Delete(product);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    return RedirectToAction(nameof(Erorr));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletAbout(int id)
        {
            var aboutItem= await _context.Set<AboutItem>().FirstOrDefaultAsync(e=>e.Id==id);
            if (aboutItem != null)
            {
                _context.Remove(aboutItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult Erorr()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ViewProduct(int id)
        {
            Product product = await _context.products.Include(a=>a.AboutItems).Include(p => p.purchaseItems).ThenInclude(p => p.Purchase).ThenInclude(s => s.Supplier).Include(i => i.Images).Include(c => c.Catigory).FirstOrDefaultAsync(p => p.Id == id);
            return View(product);
        }

        [HttpGet]   
        public async Task<IActionResult> CreateAbout(int id)
        {
            AboutItem aboutItem=new AboutItem() { ProductId=id};
            return View(aboutItem);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAbout(AboutItem aboutItem)
        {

            if (ModelState.IsValid)
            {
                aboutItem.Id = 0;
                await _context.aboutItems.AddAsync(aboutItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(aboutItem);
        }
        [HttpGet]
        public async Task<IActionResult> EditAbout(int id)
        {
            var aboutItem=await _context.Set<AboutItem>().FirstOrDefaultAsync(a => a.Id == id);
            return View(aboutItem);
        }

        [HttpPost]
        public async Task<IActionResult> EditAbout(AboutItem aboutItem)
        {
            var aboutItemdb = await _context.Set<AboutItem>().FirstOrDefaultAsync(a => a.Id == aboutItem.Id);
            if (ModelState.IsValid)
            {
                aboutItemdb.Description = aboutItem.Description;
                _context.Update(aboutItemdb);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index)); 
            }
            return View(aboutItem);
        }

        public async Task AddProductAndImage()
        {
            Image image = ConvertFromStatic.Convert(_image);
            await _ImageRepository.AddAsync(image);
        }

        public async Task<Product> GetProductAndImage(Product pro)
        {
            Product product = await _context.Set<Product>().Include(i => i.Images).FirstOrDefaultAsync(e => e.Id == pro.Id);
            product.Name = pro.Name;
            product.Price = pro.Price;
            product.Description = pro.Description;
            product.Price = pro.Price;
            product.StockQuantity = pro.StockQuantity;
            product.AcceptData = pro.AcceptData;
            product.CatigoryId=pro.CatigoryId;
            return product;
        }
        public async Task<List<Product>> GetAllProducts(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.products.Include(p=>p.purchaseItems).ThenInclude(p=>p.Purchase).ThenInclude(s=>s.Supplier).Include(i=>i.Images).Include(c=>c.Catigory).Include(a=>a.AboutItems).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Product>().Include(p => p.purchaseItems).ThenInclude(p => p.Purchase).ThenInclude(s => s.Supplier).Include(i => i.Images).Include(c => c.Catigory).Include(a=>a.AboutItems).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }

        public async Task SelectedList()
        {
            List<Catigory> catigores=await _context.Set<Catigory>().ToListAsync();
            SelectList catigoreselect = new SelectList(catigores, "Id", "Name");
            ViewBag.Catigores = catigoreselect;
            List<Supplier> suppliers=await _context.Set<Supplier>().ToListAsync();
            SelectList supplierselect=new SelectList(suppliers, "Id", "Name");
            ViewBag.Suppliers = supplierselect;

        }
    }
}
