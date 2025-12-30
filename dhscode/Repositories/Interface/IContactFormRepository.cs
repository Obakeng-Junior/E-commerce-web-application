using DHSOnlineStore.ViewModels;
using DHSOnlineStore.Models;
  // Only import once
namespace DHSOnlineStore.Repositories.Interface
{
    public interface IContactFormRepository
    {
        // Add a ContactFormSubmission to the database
        Task AddContactFormSubmissionAsync(ContactFormSubmission contactForm);

        // This method seems redundant if you're using AddContactFormSubmissionAsync
        // You could remove this or clarify the need for both methods
        Task AddContactFormAsync(ContactFormSubmission submission);

        // Get all ContactFormSubmissions from the database
        Task<IEnumerable<ContactFormSubmission>> GetAllSubmissionsAsync();
    }
}

