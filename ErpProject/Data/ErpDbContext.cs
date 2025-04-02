using ErpProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ErpProject.Data
{
    public class ErpDbContext : IdentityDbContext<AppUser>
    {
        public ErpDbContext(DbContextOptions<ErpDbContext> options) : base(options) { }
        public DbSet<Customer> customer { get; set; }
        public DbSet<Employee> employees { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<Department> departments { get; set; }
        public DbSet<Product> products { get; set; }
        public DbSet<OrderItem> orderItems { get; set; }
        public DbSet<Purchase> purchases { get; set; }
        public DbSet<PurchaseItem> purchaseItems { get; set; }
        public DbSet<SalesInvoice> salesInvoices { get; set; }
        public DbSet<Supplier> suppliers { get; set; }
        public DbSet<Catigory> catigories { get; set; }
        public DbSet<Image> images { get; set; }
        public DbSet<TotalYear> totalYears { get; set; }
        public DbSet<AboutItem> aboutItems { get; set; }
        public DbSet<Leave> leaves { get; set; }
        public DbSet<LeaveType> leaveTypes { get; set; }
        public DbSet<Incentive> incentives { get; set; }
        public DbSet<IncentiveType> incentivesTypes { get; set; }
        public DbSet<Discount> discounts { get; set; }
        public DbSet<DiscountType> discountTypes { get; set; }
        public DbSet<AttendanceAndDeparture> attendanceAndDepartures { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // ضروري لتهيئة Identity

        }
           
    }
}
