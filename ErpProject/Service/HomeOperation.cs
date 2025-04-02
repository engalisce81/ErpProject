using ErpProject.Controllers;
using ErpProject.Data;
using ErpProject.Models;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Service
{
    public class HomeOperation
    {
        private readonly ErpDbContext _context;
        public HomeOperation(ErpDbContext context)
        {
            _context = context;
        }

        public async Task YearOperation()
        {
            decimal TotalExpensive = 0;
            decimal TotalOrder=0;
            decimal TotalSales = 0;
            var yearsOld=await _context.totalYears.Select(y=>y.Year).ToListAsync();
            var Yearorders=await _context.orders.Select(o=>o.OrderDate.Year).ToListAsync();
            var Yearpurcases = await _context.purchases.Select(p => p.PurchaseDate.Year).ToListAsync();
            var years=Yearorders.Union(Yearpurcases).ToList();
            years.Sort();
            yearsOld.Sort();
            //if (years.SequenceEqual(yearsOld) == false)
            //{
                for (int i = 0; i < years.Count; i++)
                {
                    var orders = await _context.orders.Where(o => o.OrderDate.Year == years[i]).Select(o => o.TotalAmount).ToListAsync();
                    var purchases = await _context.purchases.Where(p => p.PurchaseDate.Year == years[i]).Select(p => p.TotalAmount).ToListAsync();
                    if (orders != null)
                    {
                        TotalSales = (decimal)orders.Sum();
                        TotalOrder = orders.Count();
                    }
                    if (purchases != null)
                        TotalExpensive = (decimal)purchases.Sum();

                    var year=await _context.totalYears.FirstOrDefaultAsync(y=>y.Year == years[i]);
                    if (year == null)
                    {
                    year = new TotalYear();
                        year.Year = years[i];
                        year.TotalOrder = TotalOrder;
                        year.TotalExpensive = TotalExpensive;
                        year.TotalSales = TotalSales;
                        year.TotalProfet = 0;
                        await _context.totalYears.AddAsync(year);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        year.Year = years[i];
                        year.TotalOrder = TotalOrder;
                        year.TotalExpensive = TotalExpensive;
                        year.TotalSales = TotalSales;
                        year.TotalProfet = 0;
                         _context.totalYears.Update(year);
                    _context.SaveChanges();
                    }
                }
            //}
        }

        
    }
}
