using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FAQChatbot.Controllers
{
    public class FAQController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public FAQController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }
        // In-memory FAQ data
        private readonly List<FAQ> _faqs = new List<FAQ>
        {
            new FAQ { Question = "What are your business hours?", Answer = "We are open from 9 AM to 5 PM, Monday to Friday." },
            new FAQ { Question = "Where is your company located?", Answer = "We are located in the Free State." },
            new FAQ { Question = "How can I contact customer support?", Answer = "You can contact support directlly on the website, via email at sanele.mbhele@darkhawk.co.za or call us at 068 096 4807." },
            new FAQ { Question = "What types of security services do you offer?", Answer = "We offer  a variety of security and facility management services such as Stationary Guards, Gate Guarding, CCTV Installation, Armed Reaction, Fire Evacuation, Health Emergency, Access Control, Alarm Installation, Cleaning Services, and ICT Services." },
            new FAQ { Question = "Do you provide 24/7 monitoring services?", Answer = "Yes, our monitoring services are available 24/7 for your safety." },
            new FAQ { Question = "What payment methods do you accept for your services?", Answer = "We accept credit cards, debit cards, and bank transfers." },
            new FAQ { Question = "Do you offer security consultations before installing a system?", Answer = "Yes, we provide consultations to help assess your security needs." },
            new FAQ { Question = "Can I customize the security package to fit my needs?", Answer = "Absolutely, our security packages are customizable to suit your requirements." },
            new FAQ { Question = "What is your service coverage area?", Answer = "We cover the Free State and surrounding areas." },
            new FAQ { Question = "Are there discounts available for long-term service contracts?", Answer = "Yes, we offer discounts for annual or multi-year service contracts." },
            new FAQ { Question = "Do you offer mobile security services or on-site guards?", Answer = "Yes, we provide both mobile patrols and on-site security guards." },
            new FAQ { Question = "What is included in a full security assessment?", Answer = "Our full assessment includes evaluating entry points, existing systems, and potential risks." },
            new FAQ { Question = "Is there a cancellation policy if I need to discontinue services?", Answer = "You can cancel services with a 30-day notice." },
            new FAQ { Question = "Do you have environmentally-friendly or energy-efficient equipment?", Answer = "Yes, we use eco-friendly equipment where possible." },
            new FAQ { Question = "What should I do if there’s an emergency or system malfunction?", Answer = "Contact our support team immediately, and we’ll assist you 24/7." }
        };


        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage([FromBody] ChatRequest request)
        {
            var response = GetFaqResponse(request.Message);
            return Json(new { response });
        }

        // Find the best-matching FAQ answer
        private string GetFaqResponse(string userMessage)
        {
            var match = _faqs.FirstOrDefault(f =>
                userMessage.Contains(f.Question, System.StringComparison.OrdinalIgnoreCase));

            return match != null
                ? match.Answer
                : "I'm sorry, I don't have an answer to that question.";
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
    }
}