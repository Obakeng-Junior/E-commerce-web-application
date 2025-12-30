using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : BaseController
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public OrderController(IOrderRepository orderRepository, ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {
            _orderRepository = orderRepository;
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderRepository.UserOrders();
            return View(orders);
        }

        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = GetUserId();
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();


            return View(notifications);
        }

        public async Task<IActionResult> DeleteNotification(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            return View(notification);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNotification(Notification notification)
        {
            try
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetUserNotifications");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return RedirectToAction("GetUserNotifications");
            }
        }

        private string GetUserId()
        {
            var principal = _contextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);

            return userId;
        }
    }
}