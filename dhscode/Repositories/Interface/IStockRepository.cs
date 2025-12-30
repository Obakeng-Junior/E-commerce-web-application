using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;

namespace DHSOnlineStore.Repositories.Interface
{
    public interface IStockRepository
    {
        Task<IEnumerable<StockDisplayDTO>> GetStock(string sTerm = "");
        Task<Stock?> GetStockByProductId(int productId);
        Task ManageStock(StockDTO stockToManage);
    }
}
