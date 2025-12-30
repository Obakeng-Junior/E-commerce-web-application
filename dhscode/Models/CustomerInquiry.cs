using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace DHSOnlineStore.Models
{
    public class CustomerInquiry
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // The ID of the user submitting the inquiry

        public IdentityUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string Subject { get; set; }  // Subject of the inquiry/complaint

        [Required]
        [StringLength(100)]
        public string Email { get; set; } // Email so that admin can know who sent inquiry

        [Required]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }  // Details of the inquiry/complaint

        public DateTime DateSubmitted { get; set; } = DateTime.Now;
        [Required]
        public string InquiryType { get; set; }  // e.g., "Inquiry", "Complaint", "Installation Support"

        public string Status { get; set; } = "Pending";  // Default status: "Pending"

        public string? AdminResponse { get; set; }
    }
}
