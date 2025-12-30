using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts(string sTerm = "", string category = "");
    }
}
