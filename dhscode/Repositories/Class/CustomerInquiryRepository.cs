using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Repositories.Class
{
    public class CustomerInquiryRepository : ICustomerInquiryRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomerInquiryRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        // Get all inquiries
        public async Task<IEnumerable<CustomerInquiry>> GetAllInquiriesAsync()
        {
            return await _context.CustomerInquiries
                .Include(i => i.User)  // Optional: Include User details if needed
                .ToListAsync();
        }

        // Get a specific inquiry by ID
        public async Task<CustomerInquiry> GetInquiryByIdAsync(int id)
        {
            return await _context.CustomerInquiries
                .Include(i => i.User)  // Optional: Include User details if needed
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Add a new inquiry
        public async Task AddInquiryAsync(CustomerInquiry inquiry)
        {
            await _context.CustomerInquiries.AddAsync(inquiry);
            await _context.SaveChangesAsync();
            string Message = "Your inquiry has been sent. Keep an eye on your inquiry status and admin response";
            var userId = GetUserId();
            await CreateNotification(userId, 0, Message);
        }

        public async Task<IEnumerable<CustomerInquiry>> GetInquiriesByUserIdAsync(string userId)
        {
            return await _context.CustomerInquiries
                .Where(i => i.UserId == userId)
                .ToListAsync();
        }

        // Update an existing inquiry (e.g., add admin response)
        public async Task UpdateInquiryAsync(CustomerInquiry inquiry)
        {
            _context.CustomerInquiries.Update(inquiry);
            await _context.SaveChangesAsync();
            string Message = "Your inquiry status has been updated";
            var userId = GetUserId();
            await CreateNotification(inquiry.UserId, 0, Message);
        }

        // Delete an inquiry by ID
        public async Task DeleteInquiryAsync(int id)
        {
            var inquiry = await _context.CustomerInquiries.FindAsync(id);
            if (inquiry != null)
            {
                _context.CustomerInquiries.Remove(inquiry);
                await _context.SaveChangesAsync();
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
