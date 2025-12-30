using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Web;

namespace DHSOnlineStore.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : BaseController
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICompositeViewEngine _viewEngine;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly IConfiguration _config;

        public CartController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, ICartRepository cartRepository, ICompositeViewEngine viewEngine, IConfiguration config) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _cartRepository = cartRepository;
            _viewEngine = viewEngine;
            _config = config;
        }

        public async Task<IActionResult> AddItem(int productId, int quantity = 1, int redirect = 0)
        {
            var cartCount = await _cartRepository.AddItem(productId, quantity);
            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");

        }

        public async Task<IActionResult> IncreaseItem(int productId, int quantity = 1, int redirect = 0)
        {
            var cartCount = await _cartRepository.IncreaseItem(productId, quantity);
            if (redirect == 0)
                return Ok(cartCount);

            return RedirectToAction("GetUserCart");

        }

        public async Task<IActionResult> RemoveItem(int productId)
        {
            var cartCount = await _cartRepository.RemoveItem(productId);
            return RedirectToAction("GetUserCart");
        }
        public async Task<IActionResult> GetUserCart()
        {
            var cart = await _cartRepository.GetUserCart();
            return View(cart);
        }

        [HttpGet]
        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutDTO checkout)
        {
            bool isCheckedOut = await _cartRepository.DoCheckOut(checkout);
            if (!ModelState.IsValid)
                return View(checkout);
            if (isCheckedOut == false)
            {
                TempData["errorMessage"] = "Order failed.";
                return View(checkout);
            }
            else
            {
                TempData["successMessage"] = "Order successful!";
                return RedirectToAction("Pay");
            }

        }

        [HttpGet]
        public IActionResult Pay()
        {
            // Ensure the user is signed in
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if not authenticated
            }

            // Retrieve the user's email from claims or database
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            // Fetch user's cart from database
            var cart = _context.Carts
                .Include(c => c.CartDetails)
                .ThenInclude(cd => cd.Product)
                .FirstOrDefault(c => c.UserId == userId);

            if (cart == null || !cart.CartDetails.Any())
            {
                return RedirectToAction("GetUserCart", "Cart"); // Redirect if cart is empty
            }

            // Calculate total amount
            double totalAmount = cart.CartDetails.Sum(item => item.Product.Price * item.Quantity);

            // Generate a simple item name (e.g., "Dark Hawk Security Order")
            string itemName = "Dark Hawk Security Order";

            var merchantId = _config["PayFast:MerchantId"];
            var merchantKey = _config["PayFast:MerchantKey"];
            var returnUrl = _config["PayFast:ReturnUrl"];
            var cancelUrl = _config["PayFast:CancelUrl"];
            var notifyUrl = _config["PayFast:NotifyUrl"];
            var processUrl = _config["PayFast:ProcessUrl"];

            // Prepare payment data
            var paymentData = new Dictionary<string, string>
    {
        { "merchant_id", merchantId },
        { "merchant_key", merchantKey },
        { "return_url", returnUrl },
        { "cancel_url", cancelUrl },
        { "notify_url", notifyUrl },
        { "name_first", User.Identity.Name }, // Assuming User.Identity.Name holds first name
        { "name_last", "" }, // You can extend user model to store last name
        { "email_address", userEmail ?? "no-email@darkhawk.com" }, // Use actual user email
        { "amount", totalAmount.ToString("F0") }, // Format amount correctly
        { "item_name", itemName }
    };

            // Convert data to query string
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var item in paymentData)
            {
                queryString[item.Key] = item.Value;
            }

            // Redirect to PayFast Sandbox
            return Redirect($"{processUrl}?{queryString}");

        }

        //public async Task<IActionResult> DownloadPdf()
        //{
        //    var cart = await _cartRepository.GetUserCart();
        //    // Generate HTML from the view
        //    var htmlContent = await RenderViewAsString("GetUserCart", cart);

        //    // Optionally, manipulate HTML to remove specific elements before PDF generation
        //    htmlContent = htmlContent.Replace("class='hide-from-pdf'", "style='display:none'");
        //    htmlContent = htmlContent.Replace("My Cart", "Quote - Dark Hawk Security");

        //    // Set up PDF options
        //    var pdfDocument = new HtmlToPdfDocument()
        //    {
        //        GlobalSettings = new GlobalSettings
        //        {
        //            PaperSize = PaperKind.A4,
        //            Orientation = Orientation.Portrait
        //        },
        //        Objects = {
        //        new ObjectSettings
        //        {
        //            HtmlContent = htmlContent,
        //            WebSettings = { DefaultEncoding = "utf-8" }
        //        }
        //    }
        //    };

        //    // Convert HTML to PDF
        //    var pdf = _converter.Convert(pdfDocument);

        //    // Return PDF as downloadable file
        //    return File(pdf, "application/pdf", "Quote.pdf");
        //}

        //private async Task<string> RenderViewAsString(string viewName, object model)
        //{
        //    ViewData.Model = model;
        //    using var sw = new StringWriter();

        //    // Find the view
        //    var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
        //    if (!viewResult.Success)
        //    {
        //        throw new FileNotFoundException($"View '{viewName}' not found.");
        //    }

        //    // Render the view to the StringWriter
        //    var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw, new HtmlHelperOptions());
        //    await viewResult.View.RenderAsync(viewContext);

        //    return sw.ToString();
        //}


    }
}
