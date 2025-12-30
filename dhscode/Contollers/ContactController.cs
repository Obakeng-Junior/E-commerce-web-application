using Microsoft.AspNetCore.Mvc;
using DHSOnlineStore.Models;
using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using Microsoft.AspNetCore.Identity;

public class ContactController : BaseController
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;

    public ContactController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
    }
    public IActionResult Index()
    {
        var model = new ContactViewModel
        {
            Contacts = new List<ContactViewModel>
        {
            new ContactViewModel
            {
                Title = "CEO",
                Address = "19 Blackthorn Street, Mandela View, Bloemfontein, 9301",
                PhoneNumber = "+27 68 096 4807",
                  Emails = new List<string> { "sanele.mbhele@darkhawk.co.za" },
                 WorkingHours = "Mon - Fri: 9am - 5pm"
            },
            new ContactViewModel
            {
                Title = "Sales ",
                Emails = new List<string> { "sales@darkhawk.co.za", "sales1@darkhawk.co.za" },
                Address = "19 Blackthorn Street, Mandela View, Bloemfontein, 9301",
                WorkingHours = "Mon - Fri: 9am - 5pm"
            },
              new ContactViewModel
            {
                Title = "Technical Support ",
                Emails = new List<string> { "techsupport@darkhawk.co.za" },
                Address = "19 Blackthorn Street, Mandela View, Bloemfontein, 9301",
                WorkingHours = "Mon - Fri: 9am - 5pm"
            },
            new ContactViewModel
            {
                Title = "General Enquiries",
                PhoneNumber = "+27 68 096 4807",
                 Emails = new List<string> { "info@darkhawk.co.za" },
                  Address = "19 Blackthorn Street, Mandela View, Bloemfontein, 9301",
                 WorkingHours = "Mon - Fri: 9am - 5pm"
            },
            // Add more contact details as needed
        }

        };

        return View(model);
    }
}