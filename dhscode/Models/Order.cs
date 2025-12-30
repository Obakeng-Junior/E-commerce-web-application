using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHSOnlineStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }
        [Required]
        [MaxLength(30)]
        public string? UserName { get; set; }
        [EmailAddress]
        [MaxLength(30)]
        public string? Email { get; set; }
        [Required]
        public string? MobileNumber { get; set; }
        [Required]
        [MaxLength(200)]
        public string? Address { get; set; }
        [Required]
        [MaxLength(30)]
        public string? PaymentMethod { get; set; }
        public bool IsPaid { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool IsDeleted { get; set; }
        public List<OrderDetail> OrderDetail { get; set; }
    }
}
