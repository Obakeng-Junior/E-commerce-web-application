using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Repositories.Class;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Security.Claims;
using System.Web;

namespace DHSOnlineStore.Controllers
{
    public class PayFastController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;
        private readonly ICartRepository _cartRepository;

        public PayFastController(IConfiguration config, ApplicationDbContext context, ICartRepository cartRepository)
        {
            _config = config;
            _context = context;
            _cartRepository = cartRepository;
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

        //[HttpPost]
        //public async Task<IActionResult> Checkout(CheckoutDTO checkout)
        //{
        //    bool isCheckedOut = await _cartRepository.DoCheckOut(checkout);
        //    if (!ModelState.IsValid)
        //        return View(checkout);
        //    if (isCheckedOut == false)
        //    {
        //        TempData["errorMessage"] = "Checkout failed.";
        //        return View(checkout);
        //    }
        //    else
        //    {
        //        TempData["successMessage"] = "Checkout successful!";
        //        return RedirectToAction("Pay");
        //    }

        //}


        [HttpPost]
        public IActionResult Notify()
        {
            // PayFast IPN handler (to be implemented)
            return Ok();
        }

        [HttpGet]
        public IActionResult PaymentSuccess()
        {
            return Content("Payment was successful!");
        }

        [HttpGet]
        public IActionResult PaymentCancel()
        {
            return Content("Payment was canceled!");
        }
    }
}
