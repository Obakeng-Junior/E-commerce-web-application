using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DHSOnlineStore.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return View("NoResults"); // Handle empty query case.
            }

            // Search logic for products
            var productResults = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.Id,
                    Title = p.Name,
                    Image = p.Image,
                    Description = p.Description,
                    Link = $"/Product/ProductDetails/{p.Id}"
                })
                .ToListAsync();

            // Search logic for inquiries
            var inquiryResults = await _context.CustomerInquiries
                .Where(i => i.Subject.Contains(query) || i.Message.Contains(query))
                .Select(i => new SearchResult
                {
                    Id = i.Id,
                    Title = $"Inquiry: {i.Subject}",
                    Description = i.Message,
                    Link = $"/CustomerInquiry/Details/{i.Id}"
                })
                .ToListAsync();

            // Combine product and inquiry results
            var results = productResults.Concat(inquiryResults).ToList();

            // Check if there are no results
            if (!results.Any())
            {
                return View("NoResults"); // No results found for the search.
            }

            // Return search results if there are any
            return View("SearchResults", results);
        }
    }
}