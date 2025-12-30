using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Repositories.Class
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public OrderRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task SaveOrderAsync(Order order)
        {
            _context.Orders.Add(order); // Add the order to the context
            await _context.SaveChangesAsync(); // Save changes to the database
        }
        public async Task ChangeOrderStatus(OrderStatusDTO orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderStatus.OrderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order with id: {orderStatus.OrderId} was not found");
            }
            order.OrderStatusId = orderStatus.OrderStatusId;
            await _context.SaveChangesAsync();

            orderStatus.Message = "Your order status has been updated!";

            await CreateNotification(order.UserId, order.Id, orderStatus.Message);
        }
        private async Task CreateNotification(string userId, int orderId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                OrderId = orderId,
                Message = message,
                IsRead = false,
                CreatedDate = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrderById(int Id)
        {
            return await _context.Orders.FindAsync(Id) ?? throw new ArgumentNullException();
        }

        public async Task<IEnumerable<OrderStatus>> GetOrderStatus()
        {
            return await _context.OrderStatus.ToListAsync();
        }

        public async Task TogglePaymentStatus(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"order with id: {orderId} was not found");
            }
            order.IsPaid = !order.IsPaid;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> UserOrders(bool getAll = false)
        {
            var orders = _context.Orders
                .Include(x => x.OrderStatus)
                .Include(x => x.OrderDetail)
                .ThenInclude(x => x.Product)
                .AsQueryable();

            if (!getAll)
            {
                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");
                orders = orders.Where(a => a.UserId == userId);
                return await orders.ToListAsync();
            }

            return await orders.ToListAsync();
        }

        private string GetUserId()
        {
            var principal = _contextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);

            return userId;
        }
    }
}
