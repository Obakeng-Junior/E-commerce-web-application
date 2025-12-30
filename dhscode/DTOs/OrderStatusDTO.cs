using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace DHSOnlineStore.DTOs
{
    public class OrderStatusDTO
    {
        public int OrderId { get; set; }
        [Required]
        public int OrderStatusId { get; set; }
        public IEnumerable<SelectListItem> OrderStatusList { get; set; }
        public string Message { get; set; }
    }
}
