using ErpProject.Data;
using ErpProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;


namespace ErpProject.Controllers
{
    [Authorize(Roles ="Admin")]
    public class ExcelController : Controller
    {
        private readonly ErpDbContext _context;
        public ExcelController(ErpDbContext context) 
        { 
            _context = context;
        }    
        public async Task< IActionResult> ConvertEmployee()
        {
            var employees = await _context.employees.Include(d => d.Department).Include(l=>l.Leaves).ThenInclude(lt=>lt.LeaveType).Include(d=>d.discounts).ThenInclude(dt=>dt.DiscountType).Include(i=>i.incentives).ThenInclude(it=>it.IncentiveType).Include(a=>a.attendanceAndDepartures).ToListAsync();
            var package = new ExcelPackage();
            var workSheet=package.Workbook.Worksheets.Add("Employees");
            workSheet.Cells[1, 1].Value = "Id";
            workSheet.Cells[1, 2].Value = "Name";
            workSheet.Cells[1, 3].Value = "Basic Salary";
            workSheet.Cells[1, 4].Value = "Final Salary";
            workSheet.Cells[1, 5].Value = "Hire Date";
            workSheet.Cells[1, 6].Value = "Phone";
            workSheet.Cells[1, 7].Value = "Email";
            workSheet.Cells[1, 8].Value = "Department";
            workSheet.Cells[1, 9].Value = "Leaves";
            workSheet.Cells[1, 10].Value = "Discounts";
            workSheet.Cells[1, 11].Value = "Incentives";
            workSheet.Cells[1, 12].Value = "Att And Dep";

            for(int i = 0; i < employees.Count; i++)
            {
                List<string> leaves = new List<string>();
                List<string> discounts = new List<string>();
                List<string> incentives = new List<string>();
                List<decimal?> attAndDeps = new List<decimal?>();
                workSheet.Cells[i + 2, 1].Value = employees[i].Id;
                workSheet.Cells[i + 2, 2].Value = employees[i].Name;
                workSheet.Cells[i + 2, 3].Value = employees[i].BasicSalary;
                workSheet.Cells[i + 2, 4].Value = employees[i].FinalSalary;
                workSheet.Cells[i + 2, 5].Value = employees[i].HireDate.Value.ToString("dd/MM/yyyy");
                workSheet.Cells[i + 2, 6].Value = employees[i].Phone;
                workSheet.Cells[i + 2, 7].Value = employees[i].Email;
                if (employees[i].Department != null)
                    workSheet.Cells[i + 2, 8].Value = employees[i].Department.Name;
                else
                    workSheet.Cells[i + 2, 8].Value = "Not Found";
                foreach (var leave in employees[i].Leaves)
                    leaves.Add(leave.LeaveType.Name);
                foreach (var discount in employees[i].discounts)
                    discounts.Add(discount.DiscountType.Name);
                foreach (var incentive in employees[i].incentives)
                    incentives.Add(incentive.IncentiveType.Name);
                foreach (var attendanceAndDeparture in employees[i].attendanceAndDepartures)
                    attAndDeps.Add(attendanceAndDeparture.DiscountValue);
                if(leaves.Count > 0)
                    workSheet.Cells[i + 2, 9].Value = string.Join(",", leaves);
                else
                    workSheet.Cells[i + 2, 9].Value = "Not Found";
                if(discounts.Count > 0)
                    workSheet.Cells[i + 2, 10].Value = string.Join(",", discounts);
                else
                    workSheet.Cells[i + 2, 10].Value = "Not Found";
                if(incentives.Count>0)
                    workSheet.Cells[i + 2, 11].Value = string.Join(",", incentives);
                else
                    workSheet.Cells[i + 2, 11].Value = "Not Found";
                if(attAndDeps.Count>0)
                    workSheet.Cells[i + 2, 12].Value = string.Join(",", attAndDeps);
                else
                    workSheet.Cells[i + 2, 12].Value = "Not Found";
                leaves.Clear();
                discounts.Clear();
                incentives.Clear();
                attAndDeps.Clear();
            }
            var range = workSheet.Cells[1, 1, employees.Count + 1, 12];

            // تحويل النطاق إلى جدول
            var table = workSheet.Tables.Add(range, "DataTable");
            table.TableStyle = TableStyles.Medium9;
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position=0;
            return File(stream , "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employees.xlsx") ;
        }
    }
}
