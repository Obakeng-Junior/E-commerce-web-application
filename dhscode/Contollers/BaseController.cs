using DHSOnlineStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Controllers
{
    public class BaseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public BaseController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            if (User.Identity.IsAuthenticated)
            {
                var userId = GetUserId();
                var notificationCount = _context.Notifications
                                 .Where(n => n.UserId == userId)
                                 .AsNoTracking()  
                                 .Count();


                // Set notification count in ViewData
                ViewData["NotificationCount"] = notificationCount;
            }
            else
            {
                ViewData["NotificationCount"] = 0; // Default for unauthenticated users
            }
        }

        private string GetUserId()
        {
            var principal = _contextAccessor.HttpContext.User;
            return _userManager.GetUserId(principal);
        }
    }
}