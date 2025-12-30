using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface ICartRepository
    {
        Task<int> AddItem(int productId, int quantity);
        Task<int> IncreaseItem(int productId, int quantity);

		Task<int> RemoveItem(int productId);
        Task<Cart> GetUserCart();
        Task<Cart> GetCart(string userId);
        Task<bool> DoCheckOut(CheckoutDTO checkout);
    }
}
