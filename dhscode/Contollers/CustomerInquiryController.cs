using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using System;
using System.Threading.Tasks;
using DHSOnlineStore.ViewModels;
using DHSOnlineStore.Repositories;
using DHSOnlineStore.Data;

namespace DHSOnlineStore.Controllers
{
    public class CustomerInquiryController : BaseController
    {
        private readonly ICustomerInquiryRepository _inquiryRepository;


        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomerInquiryController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, ICustomerInquiryRepository inquiryRepository) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _inquiryRepository = inquiryRepository;
        }

        // Display inquiry form to customer
        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult CreateInquiry()
        {
            return View();
        }

        // Handle form submission and save inquiry
        // POST: Customer submits an inquiry
        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInquiry(CustomerInquiry inquiry)
        {
            inquiry.UserId = _userManager.GetUserId(User);

            // Log or Debugging Step: Check if the form is correctly populated
            Console.WriteLine($"Received inquiry: Subject = {inquiry.Subject}, Message = {inquiry.Message}, InquiryType = {inquiry.InquiryType}, Id = {inquiry.Id}, UserId = {inquiry.UserId}, Email = {inquiry.Email}");

            // Check if the user is authenticated before assigning UserId
            //if (User.Identity.IsAuthenticated)
            //{
            //    // Set the UserId from the logged-in user
                
            //}

            // Set the DateSubmitted field to the current UTC time
            //inquiry.DateSubmitted = DateTime.UtcNow;

            try
            {
                // Attempt to add the inquiry to the database
                
                await _inquiryRepository.AddInquiryAsync(inquiry);
                Console.WriteLine("Inquiry added successfully");
            }
            catch (Exception ex)
            {
                // Catch any exception and log the error
                Console.WriteLine($"Error while saving inquiry: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving your inquiry. Please try again.");
                return View(inquiry);  // Return to the view with an error message
            }

            // Success message
            TempData["successMessage"] = "Your inquiry has been submitted successfully.";

            // Redirect to the Home page after successful submission
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyInquiries()
        {
            var userId = _userManager.GetUserId(User);
            var inquiries = await _inquiryRepository.GetInquiriesByUserIdAsync(userId);
            return View(inquiries);
        }


        // List inquiries for admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageInquiries()
        {
            var inquiries = await _inquiryRepository.GetAllInquiriesAsync();
            return View(inquiries);
        }

        // Admin response form
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RespondToInquiry(int id)
        {
            var inquiry = await _inquiryRepository.GetInquiryByIdAsync(id);
            if (inquiry == null)
            {
                return NotFound();
            }
            return View(inquiry);
        }


        // Save admin response
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RespondToInquiry(CustomerInquiry inquiry, string adminResponse)
        {
            var existingInquiry = await _inquiryRepository.GetInquiryByIdAsync(inquiry.Id);
            if (existingInquiry == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(adminResponse))
            {
                ModelState.AddModelError("adminResponse", "Admin response cannot be empty.");
                return View(inquiry);
            }

            existingInquiry.AdminResponse = adminResponse;
            existingInquiry.Status = "Responded";
            await _inquiryRepository.UpdateInquiryAsync(existingInquiry);

            return RedirectToAction("ManageInquiries");
        }
    }
}