using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Repositories.Class
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StockDisplayDTO>> GetStock(string sTerm = "")
        {
            sTerm = sTerm.ToLower();
            var stockList = await (from product in _context.Products
                                   join stock in _context.Stocks on product.Id
                                   equals stock.ProductId into product_stock
                                   from productStock
                                   in product_stock.DefaultIfEmpty()
                                   where string.IsNullOrEmpty(sTerm)
                                   || product.Name.ToLower().Contains(sTerm)
                                   select new StockDisplayDTO
								   {
                                       ProductId = product.Id,
                                       ProductName = product.Name,
                                       Quantity = productStock == null ? 0 : productStock.Quantity,
                                   }).ToListAsync();
                                   
            return stockList;
        }

        public async Task<Stock?> GetStockByProductId(int productId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(a => a.ProductId == productId);
        }

        public async Task ManageStock(StockDTO stockToManage)
        {
            var existingStock = await GetStockByProductId(stockToManage.ProductId);
            if (existingStock == null)
            {
                var stock = new Stock
                {
                    ProductId = stockToManage.ProductId,
                    Quantity = stockToManage.Quantity,
                };
                _context.Stocks.Add(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }

            await _context.SaveChangesAsync();
        }
    }
}
