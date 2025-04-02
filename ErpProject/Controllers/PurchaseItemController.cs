using ErpProject.Constant;
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
    [Authorize(Policy = Permissions.PurchaseItem.View)]
    public class PurchaseItemController : Controller
    {
        private static Image _image = new Image();
        private readonly IRepository<PurchaseItem> _PurchaseItemRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        public PurchaseItemController(IRepository<PurchaseItem> PurchaseItemRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _PurchaseItemRepository = PurchaseItemRepository;
            _ImageRepository = ImageRepository;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {

            var entitys = await GetAllPurchaseItems(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.PurchaseItem.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectList();
            return View();
        }
        [Authorize(Policy = Permissions.PurchaseItem.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(PurchaseItem purchaseItem)
        {  
            if (ModelState.IsValid)
            {
                purchaseItem.AcceptData = true;
                await AddOpration(purchaseItem);
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseItem);
        }

        public async Task AddOpration( PurchaseItem purchaseItem)
        {
            Purchase purchase = await _context.Set<Purchase>().Include(s => s.Supplier).FirstOrDefaultAsync(p => p.Id == purchaseItem.PurchaseId);
            Product product = await _context.Set<Product>().FirstOrDefaultAsync(p => p.Id == purchaseItem.ProductId);
            purchaseItem.TotalPrice = purchaseItem.Quantity * purchaseItem.UnitPricePurchse;
            purchaseItem.UnitPrice=product.Price;
            await _PurchaseItemRepository.AddAsync(purchaseItem);
            product.StockQuantity += purchaseItem.Quantity;
            purchase.TotalAmount += purchaseItem.TotalPrice;
            _context.SaveChanges();
            _context.Update(purchase);
            _context.Update(product);
           
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
        [Authorize(Policy = Permissions.PurchaseItem.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            await SelectList();
            PurchaseItem entity = await _PurchaseItemRepository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.PurchaseItem.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(PurchaseItem purchaseItem)
        {
            await SelectList();
         
            if (ModelState.IsValid)
            {
                await EditOperation(purchaseItem);
                return RedirectToAction(nameof(Index));
            }
            return View(purchaseItem);
        }


       public async Task EditOperation(PurchaseItem purchaseItem)
       {
            PurchaseItem purchaseItemDb = await _context.Set<PurchaseItem>().FirstOrDefaultAsync(pi => pi.Id == purchaseItem.Id);
            Purchase purchaseDb = await _context.Set<Purchase>().Include(s => s.Supplier).FirstOrDefaultAsync(p => p.Id == purchaseItemDb.PurchaseId);
            Product productDb = await _context.Set<Product>().FirstOrDefaultAsync(p => p.Id == purchaseItemDb.ProductId);
            Purchase purchase = await _context.Set<Purchase>().Include(s => s.Supplier).FirstOrDefaultAsync(p => p.Id == purchaseItem.PurchaseId);
            Product product = await _context.Set<Product>().FirstOrDefaultAsync(p => p.Id == purchaseItem.ProductId);
            purchaseItem.UnitPrice = product.Price;
            purchaseItem.TotalPrice=purchaseItem.UnitPricePurchse*purchaseItem.Quantity;

            if (product.Id != productDb.Id)
            {
                productDb.StockQuantity -= purchaseItemDb.Quantity;
                _context.Update(productDb);
                product.StockQuantity += purchaseItem.Quantity;
            }
            if (purchase.Id != purchaseDb.Id)
            {
                purchaseDb.TotalAmount -= purchaseItem.TotalPrice;
                purchase.TotalAmount += purchaseItem.Quantity * purchaseItem.UnitPricePurchse;
                _context.Update(purchaseDb);
            }
            if (purchaseItemDb.Quantity != purchaseItem.Quantity)
            {
                var Quantity= purchaseItemDb.Quantity - purchaseItem.Quantity;
                if(Quantity>0)
                    product.StockQuantity-= Quantity;
                else
                    product.StockQuantity += (-1*Quantity);
            }
            if(purchaseItemDb.TotalPrice != purchaseItem.TotalPrice)
            {
               var totalprice= purchaseItemDb.TotalPrice - purchaseItem.TotalPrice;
                if (totalprice > 0)
                    purchase.TotalAmount -= totalprice;
                else 
                    purchase.TotalAmount += (-totalprice);                
            }
            _context.Update(purchase);
            _context.Update(product);
            _PurchaseItemRepository.Update(ConvertFromVMToModel(purchaseItemDb,purchaseItem));
        }

        public PurchaseItem ConvertFromVMToModel(PurchaseItem purchaseItemDb, PurchaseItem purchaseItem)
        {
            purchaseItemDb.Quantity = purchaseItem.Quantity;
            purchaseItemDb.UnitPrice = purchaseItem.UnitPrice;
            purchaseItemDb.UnitPricePurchse = purchaseItem.UnitPricePurchse;
            purchaseItemDb.TotalPrice = purchaseItem.Quantity * purchaseItem.UnitPricePurchse;
            purchaseItemDb.PurchaseId = purchaseItem.PurchaseId;
            purchaseItemDb.ProductId = purchaseItem.ProductId;
            purchaseItemDb.AcceptData = purchaseItem.AcceptData;
            return purchaseItemDb;
        }
        [Authorize(Policy = Permissions.PurchaseItem.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            PurchaseItem purchaseItem = await _PurchaseItemRepository.GetByIdAsync(id);


            if (purchaseItem != null)
            {
                await DeletOperation(purchaseItem);
                _PurchaseItemRepository.Delete(purchaseItem);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
        
        public async Task DeletOperation(PurchaseItem purchaseItem)
        {
            Purchase purchase = await _context.Set<Purchase>().Include(s => s.Supplier).FirstOrDefaultAsync(p => p.Id == purchaseItem.PurchaseId);
            Product product = await _context.Set<Product>().FirstOrDefaultAsync(p => p.Id == purchaseItem.ProductId);
            purchaseItem.TotalPrice = purchaseItem.Quantity * purchaseItem.UnitPricePurchse;
            product.StockQuantity-=purchaseItem.Quantity;
            purchase.TotalAmount-=purchaseItem.TotalPrice;
            _context.Update(purchase);
            _context.Update(product);
            _context.SaveChanges();

        }

       
        public async Task SelectList()
        {
            var purchases = await _context.Set<Purchase>().Include(s => s.Supplier).Select(p => new PurchasVM { Id = p.Id, Name = p.Supplier.Name }).ToListAsync();
            SelectList selectListsPurchas = new SelectList(purchases, "Id", "Name", "Selected value");
            ViewBag.Purchases = selectListsPurchas;

            var products = await _context.Set<Product>().ToListAsync();
            SelectList selectListsProduct = new SelectList(products, "Id", "Name", "Selected value");
            ViewBag.Products = selectListsProduct;
        }


        public async Task<PurchaseItem> GetPurchaseItem(PurchaseItem purchase)
        {
            PurchaseItem purchaseitem = await _context.Set<PurchaseItem>().Include(o => o.Purchase).Include(p => p.Product).FirstOrDefaultAsync(e => e.Id == purchase.Id);
            purchaseitem.Quantity = purchase.Quantity;
            purchaseitem.UnitPrice = purchase.UnitPrice;
            purchaseitem.AcceptData = purchase.AcceptData;
            purchaseitem.PurchaseId = purchase.PurchaseId;
            purchaseitem.ProductId = purchase.ProductId;
            return purchaseitem;
        }
        public async Task<List<PurchaseItem>> GetAllPurchaseItems(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<PurchaseItem>().Include(p => p.Product).ThenInclude(i=>i.Images).Include(o => o.Purchase).ThenInclude(s=>s.Supplier).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<PurchaseItem>().Include(p => p.Product).ThenInclude(i => i.Images).Include(o => o.Purchase).ThenInclude(s=>s.Supplier).Where(e => e.Purchase.Supplier.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
    }
}
