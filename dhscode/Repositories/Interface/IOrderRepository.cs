using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> UserOrders(bool getAll = false);
        Task ChangeOrderStatus(OrderStatusDTO orderStatus);
        Task TogglePaymentStatus(int orderId);
        Task<Order> GetOrderById(int Id);
        Task<IEnumerable<OrderStatus>> GetOrderStatus();
        Task SaveOrderAsync(Order order);
    }
}
