using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ErpProject.Constant
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionList(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Creat",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delet"
            };
        }
        public static List<string> GenerateAllPermissions()
        {
            var allPermissions = new List<string>();
            foreach(var permission in Enum.GetValues(typeof(Module)))
            {
                allPermissions.AddRange(GeneratePermissionList(permission.ToString()));
            }
            return allPermissions;
        }

        public static class Department
        {
            public const  string View = "Permissions.Department.View"; 
            public const string Creat = "Permissions.Department.Creat";
            public const string Edit = "Permissions.Department.Edit";
            public const string Delet = "Permissions.Department.Delet";
        }

        public static class AboutItem
        {
            public const string View = "Permissions.AboutItem.View";
            public const string Creat = "Permissions.AboutItem.Creat";
            public const string Edit = "Permissions.AboutItem.Edit";
            public const string Delet = "Permissions.AboutItem.Delet";
        }
        public static class AttendanceAndDeparture
        {
            public const string View = "Permissions.AttendanceAndDeparture.View";
            public const string Creat = "Permissions.AttendanceAndDeparture.Creat";
            public const string Edit = "Permissions.AttendanceAndDeparture.Edit";
            public const string Delet = "Permissions.AttendanceAndDeparture.Delet";
        }
        public static class Catigory
        {
            public const string View = "Permissions.Catigory.View";
            public const string Creat = "Permissions.Catigory.Creat";
            public const string Edit = "Permissions.Catigory.Edit";
            public const string Delet = "Permissions.Catigory.Delet";
        }
        public static class Discount
        {
            public const string View = "Permissions.Discount.View";
            public const string Creat = "Permissions.Discount.Creat";
            public const string Edit = "Permissions.Discount.Edit";
            public const string Delet = "Permissions.Discount.Delet";
        }
        public static class DiscountType
        {
            public const string View = "Permissions.DiscountType.View";
            public const string Creat = "Permissions.DiscountType.Creat";
            public const string Edit = "Permissions.DiscountType.Edit";
            public const string Delet = "Permissions.DiscountType.Delet";
        }
        public static class Employee
        {
            public const string View = "Permissions.Employee.View";
            public const string Creat = "Permissions.Employee.Creat";
            public const string Edit = "Permissions.Employee.Edit";
            public const string Delet = "Permissions.Employee.Delet";
        }
        public static class Incentive
        {
            public const string View = "Permissions.Incentive.View";
            public const string Creat = "Permissions.Incentive.Creat";
            public const string Edit = "Permissions.Incentive.Edit";
            public const string Delet = "Permissions.Incentive.Delet";
        }
        public static class IncentiveType
        {
            public const string View = "Permissions.IncentiveType.View";
            public const string Creat = "Permissions.IncentiveType.Creat";
            public const string Edit = "Permissions.IncentiveType.Edit";
            public const string Delet = "Permissions.IncentiveType.Delet";
        }
        public static class Leave
        {
            public const string View = "Permissions.Leave.View";
            public const string Creat = "Permissions.Leave.Creat";
            public const string Edit = "Permissions.Leave.Edit";
            public const string Delet = "Permissions.Leave.Delet";
        }
        public static class LeaveType
        {
            public const string View = "Permissions.LeaveType.View";
            public const string Creat = "Permissions.LeaveType.Creat";
            public const string Edit = "Permissions.LeaveType.Edit";
            public const string Delet = "Permissions.LeaveType.Delet";
        }
        public static class Order
        {
            public const string View = "Permissions.Order.View";
            public const string Creat = "Permissions.Order.Creat";
            public const string Edit = "Permissions.Order.Edit";
            public const string Delet = "Permissions.Order.Delet";
        }
        public static class OrderItem
        {
            public const string View = "Permissions.OrderItem.View";
            public const string Creat = "Permissions.OrderItem.Creat";
            public const string Edit = "Permissions.OrderItem.Edit";
            public const string Delet = "Permissions.OrderItem.Delet";
        }
        public static class Product
        {
            public const string View = "Permissions.Product.View";
            public const string Creat = "Permissions.Product.Creat";
            public const string Edit = "Permissions.Product.Edit";
            public const string Delet = "Permissions.Product.Delet";
        }
        public static class Purchase
        {
            public const string View = "Permissions.Purchase.View";
            public const string Creat = "Permissions.Purchase.Creat";
            public const string Edit = "Permissions.Purchase.Edit";
            public const string Delet = "Permissions.Purchase.Delet";
        }

        public static class PurchaseItem
        {
            public const string View = "Permissions.PurchaseItem.View";
            public const string Creat = "Permissions.PurchaseItem.Creat";
            public const string Edit = "Permissions.PurchaseItem.Edit";
            public const string Delet = "Permissions.PurchaseItem.Delet";
        }
        public static class SalesInvoice
        {
            public const string View = "Permissions.SalesInvoice.View";
            public const string Creat = "Permissions.SalesInvoice.Creat";
            public const string Edit = "Permissions.SalesInvoice.Edit";
            public const string Delet = "Permissions.SalesInvoice.Delet";
        }
        public static class Supplier
        {
            public const string View = "Permissions.Supplier.View";
            public const string Creat = "Permissions.Supplier.Creat";
            public const string Edit = "Permissions.Supplier.Edit";
            public const string Delet = "Permissions.Supplier.Delet";
        }
        public static class Customer
        {
            public const string View = "Permissions.Supplier.View";
            public const string Creat = "Permissions.Supplier.Creat";
            public const string Edit = "Permissions.Supplier.Edit";
            public const string Delet = "Permissions.Supplier.Delet";
        }
        
    }
}
