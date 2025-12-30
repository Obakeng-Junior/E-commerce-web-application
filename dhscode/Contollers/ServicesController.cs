using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class ServicesController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public ServicesController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult StationaryGuards()
    {
        ViewData["ServiceName"] = "Stationary and Patrolling Guards";
        return View("ServiceDetails");
    }

    public IActionResult GateGuarding()
    {
        ViewData["ServiceName"] = "Gate and Boom Guarding";
        return View("ServiceDetails");
    }

    public IActionResult CCTVInstallation()
    {
        ViewData["ServiceName"] = "CCTV Installation and Monitoring";
        return View("ServiceDetails");
    }

    public IActionResult ArmedReaction()
    {
        ViewData["ServiceName"] = "Armed Reaction";
        return View("ServiceDetails");
    }

    public IActionResult FireEvacuation()
    {
        ViewData["ServiceName"] = "Fire and Evacuation Emergency";
        return View("ServiceDetails");
    }

    public IActionResult HealthEmergency()
    {
        ViewData["ServiceName"] = "Health and Accident Emergency";
        return View("ServiceDetails");
    }

    // Additional services
    public IActionResult AccessControl()
    {
        ViewData["ServiceName"] = "Access Control";
        return View("ServiceDetails");
    }

    public IActionResult AlarmInstallation()
    {
        ViewData["ServiceName"] = "Alarm Installation";
        return View("ServiceDetails");
    }

    public IActionResult CleaningServices()
    {
        ViewData["ServiceName"] = "Cleaning Services";
        return View("ServiceDetails");
    }

    public IActionResult ICTServices()
    {
        ViewData["ServiceName"] = "Information Communication Technology (ICT) Services";
        return View("ServiceDetails");
    }
}