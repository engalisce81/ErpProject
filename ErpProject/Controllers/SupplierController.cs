using ErpProject.Constant;
using ErpProject.CreatSteps;
using ErpProject.CreatSteps.Supplier;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.Service;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Supplier.View)]
    public class SupplierController : Controller
    {
        private static Supplier _supplier=new Supplier();
        private static Image _image=new Image();
        private readonly IRepository<Supplier> _SupplierRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context; 
        public SupplierController(IRepository<Supplier> SupplierRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _SupplierRepository= SupplierRepository;
            _ImageRepository = ImageRepository;
            _context= context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            var entitys=await GetAllSuppliers(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue});
        }

        [Authorize(Policy = Permissions.Supplier.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            return View();
        }
        [Authorize(Policy = Permissions.Supplier.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(StepData stepData)
        {
            if (ModelState.IsValid)
            {
                AsignData(stepData,_supplier);   
                return RedirectToAction(nameof(CreateImage));
            }
            return View(stepData);
        }

        [Authorize(Policy = Permissions.Supplier.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateImage()
        {
            if (_supplier.AcceptData) 
                return View();
            else
                return RedirectToAction(nameof(CreateData));
        }
        [Authorize(Policy = Permissions.Supplier.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateImage(StepImage stepImage)
        {
            if (ModelState.IsValid) 
            {
                await ConvertToImage.AsignImage(stepImage, _image);
                await AddSupplierAndImage();
                return RedirectToAction(nameof(CreateFinsih));
            }
            return View(stepImage);
        }

        [Authorize(Policy = Permissions.Supplier.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateFinsih()
        {
            return View();
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
        [Authorize(Policy = Permissions.Supplier.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            Supplier entity=await _SupplierRepository.GetByIdAsync(id);
            StepData stepData=AssignDataToEdit(entity);
            return View(stepData);
        }
        [Authorize(Policy = Permissions.Supplier.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(StepData stepData) 
        {
            if (ModelState.IsValid)
            {
                Supplier entity=await _SupplierRepository.GetByIdAsync(stepData.Id);
                AsignData(stepData, entity);
                _SupplierRepository.Update(entity);
                return RedirectToAction(nameof(Index));
            }
            return View(stepData);
        }

        [Authorize(Policy = Permissions.Supplier.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            StepImage stepImage = ConvertStream.ConvertFromStreamToFormFile(image);
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Supplier.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditImage(StepImage stepImage)
        {
            if(ModelState.IsValid)
            {
                Image image = await _ImageRepository.GetByIdAsync(stepImage.Id);
                await ConvertToImage.AsignImage(stepImage, image);
                _ImageRepository.Update(image);
                return RedirectToAction(nameof(Index));
            }
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Supplier.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Supplier entity =await _SupplierRepository.GetByIdAsync(id);
            if (entity != null)
            {
                try
                {
                    _SupplierRepository.Delete(entity);
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

        public void AsignData(StepData stepData,Supplier _supplier)
        {
            _supplier.Id = stepData.Id;
            _supplier.Name = stepData.Name;
            _supplier.Email = stepData.Email;
            _supplier.Description = stepData.Description;
            _supplier.Summary = stepData.Summary;
            _supplier.Phone = stepData.Phone;
            _supplier.Address = stepData.Address;
            _supplier.Comment = stepData.Comment;
            _supplier.AcceptData = true;
        }

        public StepData AssignDataToEdit(Supplier entity) 
        {
            StepData stepData = new StepData()
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                Description = entity.Description,
                Summary = entity.Summary,
                Phone = entity.Phone,
                Address = entity.Address,
                Comment = entity.Comment
            };
            return stepData;
        }
        public async Task AddSupplierAndImage()
        {
            //Image image = ConvertFromStatic.Convert(_image);
            Image image = new Image();
            image.Accepted = _image.Accepted;
            image.ContentType = _image.ContentType;
            image.Data = _image.Data;
            image.FileName = _image.FileName;
            image.Id = _image.Id;
            await _ImageRepository.AddAsync(image);
            _supplier.ImageId = image.Id;
            Supplier supplier = ConvertFromStatic.Convert(_supplier);
            await _SupplierRepository.AddAsync(supplier);   
        }
        public async Task<List<Supplier>> GetAllSuppliers(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Supplier>().Include(i => i.Image).Include(p=>p.Purchase).ThenInclude(pi=>pi.PurchaseItems).ThenInclude(pr=>pr.Product).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Supplier>().Include(i => i.Image).Include(p => p.Purchase).ThenInclude(pi => pi.PurchaseItems).ThenInclude(pr => pr.Product).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        } 

        


    }
}
