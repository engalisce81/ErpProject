using ErpProject.CreatSteps;
using ErpProject.Data;
using ErpProject.Models;
using ErpProject.Repository.Basic;
using ErpProject.Service;
using ErpProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ErpProject.CreatSteps.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ErpProject.Constant;
namespace ErpProject.Controllers
{
    [Authorize(Policy = Permissions.Employee.View)]
    public class EmployeeController : Controller
    {
        private static Employee _employee = new Employee();
        private static Image _image = new Image();
        private readonly IRepository<Employee> _EmployeeRepository;
        private readonly IRepository<Image> _ImageRepository;
        private readonly ErpDbContext _context;
        private readonly EmployeeOperations _employeeOperations;
        public EmployeeController(IRepository<Employee> EmployeeRepository, IRepository<Image> ImageRepository, ErpDbContext context)
        {
            _EmployeeRepository = EmployeeRepository;
            _ImageRepository = ImageRepository;
            _context = context;
            _employeeOperations = new EmployeeOperations(_context);
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchName)
        {
            await _employeeOperations.UpdateEmployees();
            var entitys = await GetAllEmployees(searchName);
            return View(entitys);
        }

        [HttpPost]
        public async Task<IActionResult> GetName(SearchVM searchNameVM)
        {
            return RedirectToAction(nameof(Index), new { searchName = searchNameVM.SearchValue });
        }

        [Authorize(Policy = Permissions.Employee.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateData()
        {
            await SelectList();
            return View();
        }
        [Authorize(Policy = Permissions.Employee.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateData(Employee emp)
        {
            if (ModelState.IsValid)
            {
                emp.AcceptData= true;
                _employee = emp;
                return RedirectToAction(nameof(CreateImage));
            }
            return View(emp);
        }

        [Authorize(Policy = Permissions.Employee.Creat)]
        [HttpGet]
        public async Task<IActionResult> CreateImage()
        {
            if (_employee.AcceptData)
                return View();
            else
                return RedirectToAction(nameof(CreateData));
        }
        [Authorize(Policy = Permissions.Employee.Creat)]
        [HttpPost]
        public async Task<IActionResult> CreateImage(StepImage stepImage)
        {
            if (ModelState.IsValid)
            {
                await ConvertToImage.AsignImage(stepImage, _image);
                await AddEmployeeAndImage();
                return RedirectToAction(nameof(CreateFinsih));
            }
            return View(stepImage);
        }

        [Authorize(Policy = Permissions.Employee.Creat)]
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
        [Authorize(Policy = Permissions.Employee.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditData(int id)
        {
            await SelectList();
            Employee entity = await _EmployeeRepository.GetByIdAsync(id);
            return View(entity);
        }
        [Authorize(Policy = Permissions.Employee.Edit)]
        [HttpPost]
        public async Task<IActionResult> EditData(Employee emp)
        {   
            if (ModelState.IsValid)
            { 
                _EmployeeRepository.Update(emp);
                return RedirectToAction(nameof(Index));
            }
            return View(emp);
        }

        [Authorize(Policy = Permissions.Employee.Edit)]
        [HttpGet]
        public async Task<IActionResult> EditImage(int id)
        {
            Image image = await _ImageRepository.GetByIdAsync(id);
            StepImage stepImage = ConvertStream.ConvertFromStreamToFormFile(image);
            return View(stepImage);
        }
        [Authorize(Policy = Permissions.Employee.Edit)]
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
        [Authorize(Policy = Permissions.Employee.Delet)]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Employee entity = await _EmployeeRepository.GetByIdAsync(id);
            if (entity != null)
            {
                try
                {
                    _EmployeeRepository.Delete(entity);
                    return RedirectToAction(nameof(Index));
                }catch(Exception ex)
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

        public async Task SelectList()
        {
            var departments=await _context.Set<Department>().ToListAsync();
            SelectList selectLists = new SelectList(departments,"Id","Name","Selected value");
            ViewBag.Departments=selectLists;
        } 
        public async Task AddEmployeeAndImage()
        {
            Image image = ConvertFromStatic.Convert(_image);
            await _ImageRepository.AddAsync(image);
            _employee.ImageId = image.Id;
            await _EmployeeRepository.AddAsync(_employee);
        }

        public async Task<Employee> GetEmployeeAndImage(Employee emp)
        {
            Employee employee=await _context.Set<Employee>().Include(i=>i.Image).FirstOrDefaultAsync(e=>e.Id==emp.Id);
            employee.Name= emp.Name;
            employee.HireDate= emp.HireDate;
            employee.Phone= emp.Phone;
            employee.Email= emp.Email;
            employee.Address= emp.Address;
            employee.Comment= emp.Comment;
            employee.AcceptData= emp.AcceptData;
            employee.DepartmentId= emp.DepartmentId;
            return employee;
        }
        public async Task<List<Employee>> GetAllEmployees(string? searcName)
        {
            if (searcName.IsNullOrEmpty())
            {
                var entites = await _context.Set<Employee>().Include(i => i.Image).Include(d => d.Department).ToListAsync();
                return entites;
            }
            else
            {
                var entites = await _context.Set<Employee>().Include(i => i.Image).Include(d=>d.Department).Where(e => e.Name.ToLower().Contains(searcName.ToLower())).ToListAsync();
                return entites;
            }
        }
    }
}
