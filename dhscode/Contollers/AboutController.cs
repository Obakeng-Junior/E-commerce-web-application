using Microsoft.AspNetCore.Mvc;
using DHSOnlineStore.Models;
using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using Microsoft.AspNetCore.Identity;

public class AboutController : BaseController
{

    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public AboutController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    public IActionResult Index()
    {
        var model = new AboutViewModel
        {
            CompanyDescription = "Dark Hawk Security (PTY) LTD is a newly registered company based in Bloemfontein, Free State, with a vision to provide top-quality security services locally and globally." +
            "The founder of Dark Hawk Security has over 14 years in Senior Management and further acquired training in safe guarding, armed\r\n    reaction accreditation and continues to acquire more accreditation as security technology improves every moment.\n " +
            "Dark Hawk Security has a vision to be one of the most recommended security companies in South Africa within 5 years. We believe\r\n    through investors and BRICS opportunities we can reach Africa and world wide.Dark Hawk Security will grow it’s employee base according to client needs, contracts, tenders for both full time and part time staff. " +
            "A\r\n    competent and dedicated management team will be set in place to ensure all operations run to exceed with excellence beyond our\r\n    clients expectations.Dark Hawk Security will implement all needed and necessary infrastructures to ensure the business achieves its vision.",
            Mission = "To be the best security company that provides exceptional quality service to our clients.",
            Vision = "To be one of the most recommended security companies in South Africa within 5 years.",
            Values = new List<string>
            {
                "Excellence",
                "Professionalism",
                "Accuracy",
                "Determination",
                "Discipline"
            }
        };

        return View(model);
    }
}