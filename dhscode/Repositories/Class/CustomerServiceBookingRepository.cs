using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static DHSOnlineStore.Models.CustomerServiceBooking;

namespace DHSOnlineStore.Repositories.Class
{
    public class CustomerServiceBookingRepository : ICustomerServiceBooking
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;
        public CustomerServiceBookingRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task AddServiceBooking(CustomerServiceBooking booking)
        {
            _context.CustomerServiceBookings.Add(booking);
            await _context.SaveChangesAsync();
            string Message = "Your service has been booked. Keep an eye on your booking status";
            var userId = GetUserId();
            await CreateNotification(userId, 0, Message);
        }

        public async Task LogBookingStatusChange(int bookingId, string status, string? reason = null)
        {
            var bookingStatusHistory = new BookingStatusHistory
            {
                BookingId = bookingId,
                Status = status,
                Reason = reason,
                ChangeDate = DateTime.UtcNow  // You may want to use the appropriate time zone
            };

            await _context.BookingStatusHistories.AddAsync(bookingStatusHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CustomerServiceBooking>> GetServiceBookingsByUser(string userId)
        {
            return await _context.CustomerServiceBookings.Where(b => b.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<CustomerServiceBooking>> GetAllServiceBookings()
        {
            return await _context.CustomerServiceBookings.ToListAsync();
        }
        public async Task<CustomerServiceBooking> GetServiceBookingById(int bookingId) // Implement this method
        {
            return await _context.CustomerServiceBookings
                        .Include(b => b.User) // Include user for email
                        .FirstOrDefaultAsync(b => b.Id == bookingId);
        }

        public async Task UpdateBookingStatus(int bookingId, string status)
        {
            var booking = await _context.CustomerServiceBookings.FindAsync(bookingId);
            if (booking != null)
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
                string Message = "Your booking status has been updated";
                var userId = GetUserId();
                await CreateNotification(booking.UserId, 0, Message);
            }
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

        private string GetUserId()
        {
            var principal = _contextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);

            return userId;
        }
    }
}
