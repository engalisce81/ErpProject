using ErpProject.Data;
using ErpProject.Models;
using ErpProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ErpProject.Service
{
    public class OrderOperations
    {
        private readonly ErpDbContext _context;
        public OrderOperations(ErpDbContext context)
        {
            _context = context;
        }

        public async Task AddOperation(OrderItem orderItem)
        {
            Order order = await _context.Set<Order>().Include(oi => oi.OrderItems).FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
            Product product = await _context.Set<Product>().FirstOrDefaultAsync(o => o.Id == orderItem.ProductId);
            if (product.StockQuantity > 0)
            {
                if (product.StockQuantity < orderItem.Quantity)
                {
                    orderItem.Quantity = product.StockQuantity;
                }
                orderItem.UnitPrice = product.Price;
                orderItem.TotalPric = orderItem.Quantity * orderItem.UnitPrice;
                if (orderItem.State)
                {
                    product.StockQuantity -= orderItem.Quantity;
                    if (product.StockQuantity < 0)
                    {
                        orderItem.Quantity += product.StockQuantity;
                        orderItem.TotalPric = orderItem.Quantity * orderItem.UnitPrice;
                        product.StockQuantity = 0;
                    }
                    order.TotalAmount += orderItem.TotalPric;
                    _context.Set<Order>().Update(order);
                    _context.Set<Product>().Update(product);
                    _context.SaveChanges();
                }               
            }
            else
            {
                orderItem.UnitPrice=product.Price;
                orderItem.TotalPric = 0;
                orderItem.Quantity = 0;
                orderItem.State = false;
            }
            await _context.AddAsync(orderItem);
            await _context.SaveChangesAsync();            
        }

        public async Task UpdateOperation(OrderItem orderItem)
        {
            OrderItem orderItemDb=await _context.orderItems.FirstOrDefaultAsync(oi=>oi.Id == orderItem.Id);
            Order order = await _context.Set<Order>().Include(oi => oi.OrderItems).FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
            Product product = await _context.Set<Product>().FirstOrDefaultAsync(o => o.Id == orderItem.ProductId);
            if (product.StockQuantity > 0)
            {
                if (product.StockQuantity < orderItem.Quantity)
                {
                    orderItem.Quantity = product.StockQuantity;
                }
                orderItem.UnitPrice = product.Price;
                orderItem.TotalPric = orderItem.Quantity * orderItem.UnitPrice;                             
                product.StockQuantity -= orderItem.Quantity;
                order.TotalAmount += orderItem.TotalPric;
                _context.products.Update(product);
                _context.orders.Update(order);  
            }
            else
            {
                orderItem.UnitPrice = product.Price;
                orderItem.TotalPric = 0;
                orderItem.Quantity = 0;
                orderItem.State = false;
            }
            orderItemDb = ConvertFromVMToModel(orderItemDb, orderItem);
            _context.orderItems.Update(orderItemDb);
            _context.SaveChanges();

        }

        public void TotalPricOperation(OrderItem orderItemDb, OrderItem orderItem, Order order)
        {
            if (orderItemDb.TotalPric > orderItem.TotalPric)
            {
                var lostPrice = orderItemDb.TotalPric - orderItem.TotalPric;
                order.TotalAmount -= lostPrice;
            }
            else if (orderItemDb.TotalPric < orderItem.TotalPric)
            {
                var AddPrice = orderItem.TotalPric - orderItemDb.TotalPric;
                order.TotalAmount += AddPrice;
            }
        }

        public void Quantity(OrderItem orderItemDb,OrderItem orderItem,Product product)
        {
            if (orderItemDb.Quantity > orderItem.Quantity)
            {
                var lostQantity = orderItemDb.Quantity - orderItem.Quantity;
                orderItem.TotalPric = orderItem.Quantity * orderItem.UnitPrice;

                product.StockQuantity += lostQantity;
            }
            else if (orderItemDb.Quantity < orderItem.Quantity)
            {
                var IncreseQantity = orderItem.Quantity - orderItemDb.Quantity;
                product.StockQuantity -= IncreseQantity;
            }
        }
        public OrderItem ConvertFromVMToModel(OrderItem orderItemDb, OrderItem orderItem)
        {
            orderItemDb.Quantity = orderItem.Quantity;
            orderItemDb.UnitPrice = orderItem.UnitPrice;
            orderItemDb.TotalPric = orderItem.Quantity * orderItem.UnitPrice;
            orderItemDb.OrderId = orderItem.OrderId;
            orderItemDb.ProductId = orderItem.ProductId;
            orderItemDb.State = orderItem.State;
            return orderItemDb;
        }

        //public async Task UpdateOrderItemAndOrderAndProudect(OrderItem orderItem)
        //{

        //    OrderItem orderItemDb = await _context.Set<OrderItem>().FirstOrDefaultAsync(oi => oi.Id == orderItem.Id);
        //    Order order = await _context.Set<Order>().Include(oi => oi.OrderItems).FirstOrDefaultAsync(o => o.Id == orderItem.OrderId);
        //    Product product = await _context.Set<Product>().FirstOrDefaultAsync(o => o.Id == orderItem.ProductId);
        //    orderItem.UnitPrice = product.Price;
        //    if (orderItemDb.State && orderItem.State)
        //    {

        //        if (orderItemDb.Quantity != orderItem.Quantity)
        //        {
        //            if (orderItemDb.Quantity > orderItem.Quantity)
        //            {
        //                var lostQantity = orderItemDb.Quantity - orderItem.Quantity;
        //                orderItem.TotalPric = orderItem.Quantity * orderItem.UnitPrice;

        //                product.StockQuantity += lostQantity;
        //            }
        //            else if (orderItemDb.Quantity < orderItem.Quantity)
        //            {
        //                var lostQantity = orderItem.Quantity - orderItemDb.Quantity;
        //                product.StockQuantity -= lostQantity;
        //            }
        //            _context.Set<Product>().Update(product);
        //            _context.SaveChanges();
        //        }

        //        if (orderItemDb.TotalPric != orderItem.TotalPric)
        //        {
        //            if (orderItemDb.TotalPric > orderItem.TotalPric)
        //            {
        //                var lostPrice = orderItemDb.TotalPric - orderItem.TotalPric;
        //                order.TotalAmount -= lostPrice;
        //            }
        //            else if (orderItemDb.TotalPric < orderItem.TotalPric)
        //            {
        //                var AddPrice = orderItem.TotalPric - orderItemDb.TotalPric;
        //                order.TotalAmount += AddPrice;
        //            }
        //            _context.Set<Order>().Update(order);
        //            _context.SaveChanges();

        //        }
        //        orderItemDb = ConvertFromVMToModel(orderItemDb, orderItem);
        //        _OrderItemRepository.Update(orderItemDb);
        //    }
        //    else
        //    {
        //        await AddOperation(orderItem);
        //    }

        //}
    }
}
