using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface ICustomerInquiryRepository
    {
        Task<IEnumerable<CustomerInquiry>> GetAllInquiriesAsync();  // Get all inquiries (for admin)
        Task<IEnumerable<CustomerInquiry>> GetInquiriesByUserIdAsync(string userId);
        Task<CustomerInquiry> GetInquiryByIdAsync(int id);          // Get a specific inquiry by ID
        Task AddInquiryAsync(CustomerInquiry inquiry);              // Add a new inquiry
        Task UpdateInquiryAsync(CustomerInquiry inquiry);           // Update an inquiry (e.g., respond)
        Task DeleteInquiryAsync(int id);
    }
}
