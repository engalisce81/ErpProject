using ErpProject.Data;
using ErpProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;

namespace ErpProject.Service
{
    public class DiscountOperations
    {
        private readonly ErpDbContext _context;

        public DiscountOperations(ErpDbContext context) 
        {
            _context = context;
        }

        public void Opearation(AttendanceAndDeparture attendanceAndDeparture)
        {
            var hours=attendanceAndDeparture.TimeOut.Hour - attendanceAndDeparture.TimeIn.Hour;
            if (hours < 8) 
                attendanceAndDeparture.DiscountValue=attendanceAndDeparture.HourLate*hours;
            else
                attendanceAndDeparture.DiscountValue = 0; 
        }
    }
}
