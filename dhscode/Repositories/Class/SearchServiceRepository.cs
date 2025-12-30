using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DHSOnlineStore.Repositories.Class
{
    public class SearchServiceRepository : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchServiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SearchResult>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<SearchResult>();

            // Example search logic
            var productResults = await _context.Products
                .Where(p => p.Name.Contains(query) || p.Description.Contains(query))
                .Select(p => new SearchResult
                {
                    Id = p.Id,
                    Title = p.Name,
                    Description = p.Description,
                    Link = $"/Products/Details/{p.Id}"
                })
                .ToListAsync();

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

            return productResults.Concat(inquiryResults).ToList();
        }
    }
}
