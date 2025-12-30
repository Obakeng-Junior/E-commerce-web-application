using DHSOnlineStore.Email;
using DHSOnlineStore.EmailService;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using sib_api_v3_sdk.Api;
//using sib_api_v3_sdk.Client;
//using sib_api_v3_sdk.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using DHSOnlineStore.Data;
using DHSOnlineStore.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace DHSOnlineStore.Controllers
{
    public class CustomerServiceBookingController : BaseController
    {
        private readonly ICustomerServiceBooking _serviceBookingRepository;
        //private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;


        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CustomerServiceBookingController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, ICustomerServiceBooking serviceBookingRepository/*, IEmailService emailService*/, IConfiguration configuration) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _serviceBookingRepository = serviceBookingRepository;
            //_emailService = emailService;
            _configuration = configuration;
        }

        // GET: Customer booking form
        [Authorize(Roles = "User")]
        public IActionResult CreateBooking()
        {
            return View();
        }

        // POST: Customer submits booking request
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateBooking(CustomerServiceBooking booking)
        {
            booking.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.Status = "Pending";
            await _serviceBookingRepository.AddServiceBooking(booking);
            return RedirectToAction("BookingConfirmation", new { bookingId = booking.Id });
        }

        [HttpGet]
        public async Task<IActionResult> BookingConfirmation(int bookingId)
        {
            var booking = await _serviceBookingRepository.GetServiceBookingById(bookingId);
            if (booking == null)
            {
                return NotFound("Booking not found");
            }

            return View(booking);
        }

        // GET: Admin view to manage bookings
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageBookings()
        {
            var bookings = await _serviceBookingRepository.GetAllServiceBookings();
            return View(bookings);
        }

        // POST: Admin updates booking status and notifies the customer
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, string status, string reason)
        {
            // Check if status is provided and if it's a valid status
            if (string.IsNullOrWhiteSpace(status))
            {
                ModelState.AddModelError("Status", "Status cannot be empty.");
                return RedirectToAction("ManageBookings");
            }

            // If the status is "Rejected" or "Canceled", ensure that a reason is provided
            if ((status == "Rejected" || status == "Canceled") && string.IsNullOrWhiteSpace(reason))
            {
                ModelState.AddModelError("Reason", "Please provide a reason for rejection or cancellation.");
                return RedirectToAction("ManageBookings");
            }

            // Update the booking status in the database
            await _serviceBookingRepository.UpdateBookingStatus(bookingId, status);

            // Log the status change in the history
            await _serviceBookingRepository.LogBookingStatusChange(bookingId, status, reason);

            var booking = await _serviceBookingRepository.GetServiceBookingById(bookingId);

            // Check if the booking and customer email are valid
            if (booking != null && booking.User != null && !string.IsNullOrWhiteSpace(booking.User.Email))
            {
                var customerEmail = booking.User.Email;
                //var subject = "Service Booking Status Update";
                var body = $"Dear {booking.User.UserName},<br><br>Your booking status for '{booking.ServiceType}' has been updated to <strong>{status}</strong>.<br><br>Thank you for choosing our service.<br>DHS Online Store";

                // Add reason if the status is "Rejected" or "Canceled"
                if (status == "Rejected" || status == "Canceled")
                {
                    body += $"<br>Reason: {reason}";
                }

                body += "<br><br>Thank you for choosing our service.<br>DHS Online Store";

                //try
                //{
                //    // Send email using Sendinblue API
                //    await SendApprovalEmailAsync(bookingId, customerEmail, subject, body);
                //}
                //catch (Exception ex)
                //{
                //    TempData["Error"] = $"Unable to send email notification. Error: {ex.Message}";
                //}
            }
            else
            {
                TempData["Error"] = "Customer email not found. Notification could not be sent.";
            }

            // Redirect back to the booking management page
            return RedirectToAction("ManageBookings");
        }


        [Authorize(Roles = "User")]
        public async Task<IActionResult> MyBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _serviceBookingRepository.GetServiceBookingsByUser(userId);
            return View(bookings);
        }

        // Send email using Sendinblue API
        //private async Task SendApprovalEmailAsync(int bookingId, string toEmail, string subject, string body)
        //{
        //    var booking = await _serviceBookingRepository.GetServiceBookingById(bookingId);
        //    if (booking == null || booking.User == null)
        //    {
        //        throw new Exception("Booking or associated user not found");
        //    }

        //    try
        //    {
        //        var apiKey = _configuration["SmtpSettings:ApiKey"]; // Use the SMTP key as the API key
        //        if (string.IsNullOrEmpty(apiKey))
        //        {
        //            throw new Exception("API key not found in configuration.");
        //        }

        //        Configuration.Default.ApiKey["api-key"] = apiKey; // Ensure that the API key is set

        //        var apiInstance = new TransactionalEmailsApi();
        //        var sender = new SendSmtpEmailSender("DHS Online Store", "no-reply@dhsstore.com");
        //        var recipient = new SendSmtpEmailTo(toEmail, booking.User.UserName);

        //        var htmlContent = $"<html><body>{body}</body></html>";
        //        var replyTo = new SendSmtpEmailReplyTo("support@dhsstore.com", "DHS Support");

        //        var email = new SendSmtpEmail(sender, new List<SendSmtpEmailTo> { recipient }, null, null, htmlContent, null, subject, replyTo);

        //        // Send the email via Sendinblue API
        //        var result = await apiInstance.SendTransacEmailAsync(email);

        //        // Log the result
        //        Console.WriteLine($"Email sent: {result.ToJson()}");
        //    }
        //    catch (ApiException apiEx)
        //    {
        //        // Log and display more detailed information about the API exception
        //        Console.WriteLine($"API error: {apiEx.Message}");
        //        TempData["Error"] = $"API error: {apiEx.Message}";
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the general exception
        //        Console.WriteLine($"Error sending email: {ex.Message}");
        //        TempData["Error"] = $"Error sending email: {ex.Message}";
        //    }
        //}
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BookingHistory(int bookingId)
        {
            // Retrieve the status history for the booking
            var statusHistory = await _context.BookingStatusHistories
                .Where(h => h.BookingId == bookingId)
                .OrderByDescending(h => h.ChangeDate)  // Sort by most recent changes
                .ToListAsync();

            if (statusHistory == null || !statusHistory.Any())
            {
                TempData["Error"] = "No booking history found for this booking.";
                return RedirectToAction("ManageBookings");
            }

            // Retrieve the booking details
            var booking = await _context.CustomerServiceBookings
                .Include(b => b.User)  // Including the User to show their information if needed
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                TempData["Error"] = "Booking not found.";
                return RedirectToAction("ManageBookings");
            }

            // Prepare the view model
            var viewModel = new BookingHistoryViewModel
            {
                Booking = booking,
                StatusHistory = statusHistory
            };

            // Pass the view model to the view
            return View(viewModel);
        }

    }
}