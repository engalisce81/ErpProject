using ErpProject.Data;
using Microsoft.EntityFrameworkCore;
namespace ErpProject.Service
{
    public class EmployeeOperations
    {
        private readonly ErpDbContext _context;
        public EmployeeOperations(ErpDbContext context)
        {
            _context = context;
        }

        public async Task UpdateEmployees()
        {
            var employees=await _context.employees.Include(d=>d.discounts).Include(l=>l.Leaves).Include(I=>I.incentives).Include(add=>add.attendanceAndDepartures).ToListAsync();
            foreach(var emp in employees)
            {
                emp.FinalSalary=emp.BasicSalary;
                foreach(var discount in emp.discounts)
                    emp.FinalSalary-=discount.TotalDiscount;
                foreach(var leave in emp.Leaves)
                    emp.FinalSalary-=leave.TotalDiscount;
                foreach (var attendanceAndDepartures in emp.attendanceAndDepartures)
                    emp.FinalSalary -= attendanceAndDepartures.DiscountValue;
                foreach (var incentive in emp.incentives)
                    emp.FinalSalary += incentive.IncentiveValue;
                _context.Update(emp);
            }
            await _context.SaveChangesAsync();
        } 

    }
}
