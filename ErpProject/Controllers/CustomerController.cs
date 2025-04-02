using ErpProject.CreatSteps;
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
using ErpProject.Constant;

namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Customer.View)]
    public class CustomerController : Controller
    {
        private static Customer _customer = new Customer();
        private static Image _image = new Image();
        private readonly IRepository<Customer> _CustomerRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        public CustomerController(IRepository<Customer> CustomerRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _CustomerRepository = CustomerRepository;
            _ImageRepository = ImageRepository;
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {

            var entitys = await GetAllCustomers(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Customer.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
         
            return View();
        }
        [Authorize(Policy = Permissions.Customer.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Customer cust)
        {
            if (ModelState.IsValid)
            {
                cust.AcceptData = true;
                _customer = cust;
                return RedirectToAction(nameof(CreateImage));
            }
            return View(cust);
        }
        [Authorize(Policy = Permissions.Customer.Creat)]

        [HttpGet]
        public async Task<IActionResult> CreateImage()
        {
            if (_customer.AcceptData)
                return View();
            else
                return RedirectToAction(nameof(CreateData));
        }
        [Authorize(Policy = Permissions.Customer.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateImage(StepImage stepImage)
        {
            if (ModelState.IsValid)
            {
                await ConvertToImage.AsignImage(stepImage, _image);
                await AddCustomerAndImage();
                return RedirectToAction(nameof(CreateFinsih));
            }
            return View(stepImage);
        }

        [Authorize(Policy = Permissions.Customer.Creat)]
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
        [Authorize(Policy = Permissions.Customer.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
           
            Customer entity = await _CustomerRepository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Customer.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Customer cust)
        {
            Customer customer = await GetCustomerAndImage(cust);
            if (ModelState.IsValid)
            {
                _CustomerRepository.Update(customer);
                return RedirectToAction(nameof(Index));
            }
            return View(cust);
        }

        [Authorize(Policy = Permissions.Customer.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            StepImage stepImage = ConvertStream.ConvertFromStreamToFormFile(image);
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Customer.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditImage(StepImage stepImage)
        {
            if (ModelState.IsValid)
            {
                Image image = await _ImageRepository.GetByIdAsync(stepImage.Id);
                await ConvertToImage.AsignImage(stepImage, image);
                _ImageRepository.Update(image);
                return RedirectToAction(nameof(Index));
            }
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Customer.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Customer entity = await _CustomerRepository.GetByIdAsync(id);
            if (entity != null)
            {
                try
                {
                    _CustomerRepository.Delete(entity);
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


        public async Task AddCustomerAndImage()
        { 
            Image image = ConvertFromStatic.Convert(_image);
            await _ImageRepository.AddAsync(image);
            _customer.ImageId = image.Id;
            Customer cust = _customer;
            await _CustomerRepository.AddAsync(cust);
        }

        public async Task<Customer> GetCustomerAndImage(Customer cust)
        {
            Customer customer = await _context.Set<Customer>().Include(i => i.Image).FirstOrDefaultAsync(e => e.Id == cust.Id);
            customer.Name = cust.Name;
            customer.Phone = cust.Phone;
            customer.Email = cust.Email;
            customer.Address = cust.Address;
            customer.AcceptData = cust.AcceptData;
            return customer;
        }
        public async Task<List<Customer>> GetAllCustomers(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Customer>().Include(i=>i.Image).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Customer>().Include(i => i.Image).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
    }
}


