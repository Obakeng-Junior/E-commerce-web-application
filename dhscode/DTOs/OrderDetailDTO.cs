using DHSOnlineStore.Models;

namespace DHSOnlineStore.DTOs
{
    public class OrderDetailDTO
    {
        public string DivId { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
    }
}
