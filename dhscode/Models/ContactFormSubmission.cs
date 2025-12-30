using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHSOnlineStore.Models
{
    public class ContactFormSubmission
    {
        public int Id { get; set; }


        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string ContactNumber { get; set; }

        public string? AccountNumber { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateSubmitted { get; set; }
        public string? Response { get; set; }

        public DateTime? DateResponded { get; set; }
    }
}
