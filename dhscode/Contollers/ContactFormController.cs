using DHSOnlineStore.Repositories.Interface;
using DHSOnlineStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DHSOnlineStore.Models;
using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DHSOnlineStore.Repositories;

public class ContactFormController : BaseController
{
    private readonly IContactFormRepository _contactFormRepository;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<ContactFormRepository> _logger;


    public ContactFormController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IContactFormRepository contactFormRepository, ILogger<ContactFormRepository> logger) : base(context, userManager, contextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _contextAccessor = contextAccessor;
        _contactFormRepository = contactFormRepository;
        _logger = logger;
    }

    // GET: Display the Contact Form
    public IActionResult ContactForm()
    {
        return View();
    }

    // POST: Handle form submission
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitContactForm(ContactFormSubmission submission)
    {
        // Log the incoming submission
        Console.WriteLine($"First Name: {submission.FirstName}");
        Console.WriteLine($"Last Name: {submission.LastName}");
        Console.WriteLine($"Email: {submission.Email}");
        Console.WriteLine($"Message: {submission.Message}");

        var userId = _userManager.GetUserId(User);
        Console.WriteLine($"User ID: {userId}");
        if (string.IsNullOrEmpty(userId))
        {
            TempData["errorMessage"] = "You must be logged in to submit the contact form.";
            return RedirectToAction("Login", "Account");
        }

        if (!ModelState.IsValid)
        {
            var errorMessages = string.Join(", ", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));

            Console.WriteLine($"Validation errors: {errorMessages}");
            // Return the form with the data and errors
        }

        // Log to check if we reach here
        Console.WriteLine("Saving submission to the database.");

        // Associate with logged-in user and save the submission
        submission.UserId = userId;
        submission.DateSubmitted = DateTime.UtcNow;

        try
        {
            await _contactFormRepository.AddContactFormAsync(submission);
            TempData["successMessage"] = "Message sent successfully!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while saving submission: {ex.Message}");
            TempData["errorMessage"] = "There was an error processing your request. Please try again later.";
            return View("ContactForm", submission);
        }
        return View("ContactForm");
    }




    // GET: Display the user's submissions
    [Authorize]
    public async Task<IActionResult> MySubmissions()
    {
        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["errorMessage"] = "You must be logged in to view your submissions.";
            return RedirectToAction("Login", "Account");
        }

        var submissions = await _context.ContactFormSubmissions
            .Where(s => s.UserId == userId) // Match only the logged-in user's submissions
            .OrderByDescending(s => s.DateSubmitted)
            .ToListAsync();

        return View(submissions);
    }

    // GET: Admin view of all submissions
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        // Get all submissions from the repository
        var submissions = await _contactFormRepository.GetAllSubmissionsAsync();

        // Create a new view model and assign the list of submissions
        var viewModel = new ContactFormViewModel
        {
            Submissions = submissions // Passing List<ContactFormSubmission> directly to the view model
        };

        return View(viewModel);
    }

    // GET: Respond to a specific submission
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RespondToSubmission(int id)
    {
        var submission = await _context.ContactFormSubmissions.FindAsync(id);
        if (submission == null)
        {
            return NotFound();
        }
        return View(submission); // Return the view to display the response form
    }

    // POST: Admin respond to a specific submission
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RespondToSubmission(int id, string response)
    {
        var submission = await _context.ContactFormSubmissions.FindAsync(id);
        if (submission == null)
        {
            return NotFound();
        }

        submission.Response = response;
        submission.DateResponded = DateTime.UtcNow;

        _context.Update(submission);
        await _context.SaveChangesAsync();

        TempData["successMessage"] = "Response saved successfully.";
        return RedirectToAction("Index");
    }
}
