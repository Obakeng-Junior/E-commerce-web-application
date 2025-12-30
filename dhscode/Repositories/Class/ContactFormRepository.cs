using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DHSOnlineStore.Repositories
{
    public class ContactFormRepository : IContactFormRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public ContactFormRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task AddContactFormAsync(ContactFormSubmission submission)
        {
            _context.ContactFormSubmissions.Add(submission);
            await _context.SaveChangesAsync();
            string Message = "Your message has been sent. Our team will be in contact with you soon";
            var userId = GetUserId();
            await CreateNotification(userId, 0, Message);
        }

        // Implement AddContactFormSubmissionAsync to add a submission to the database
        public async Task AddContactFormSubmissionAsync(ContactFormSubmission contactForm)
        {
            _context.ContactFormSubmissions.Add(contactForm);
            await _context.SaveChangesAsync();
        }

        // Implement GetAllSubmissionsAsync to fetch all submissions
        public async Task<IEnumerable<ContactFormSubmission>> GetAllSubmissionsAsync()
        {
            // Ensure the return type matches IEnumerable<ContactFormSubmission>
            return await _context.ContactFormSubmissions.ToListAsync();  // ToListAsync returns a List, but List implements IEnumerable
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
