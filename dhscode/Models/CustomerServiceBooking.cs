using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHSOnlineStore.Models
{
    public class CustomerServiceBooking
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser User { get; set; }
        
        public string? Address { get; set; }
        [Required]
        public string Product { get; set; }
        [Required]
        public string ServiceType { get; set; } // e.g., "Installation", "Repair"
        [Required]
        public DateTime PreferredDate { get; set; }
        [Required]
        public DateTime PreferredPickupOrDeliveryDate { get; set; }
        [Required]
        public string CustomerContact { get; set; } // Customer contact info
        
        public string Status { get; set; } // e.g., "Pending", "Confirmed", "Completed"
       
        public string? RejectionReason { get; set; }
        public string? PaymentStatus { get; set; }
        public List<BookingStatusHistory> StatusHistory { get; set; } // Add this to track status changes

        public class BookingStatusHistory
        {
            public int Id { get; set; }
            public int BookingId { get; set; }
            public string Status { get; set; }
            public string? Reason { get; set; }  // Optional reason for status change
            public DateTime ChangeDate { get; set; }
        }

    }
}
