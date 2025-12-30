using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DHSOnlineStore.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }
        public ICollection<CartDetail> CartDetails { get; set; }
    }
}
