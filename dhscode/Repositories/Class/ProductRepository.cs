using DHSOnlineStore.Data;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Repositories.Class
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts(string sTerm = "", string category = "")
        {
            sTerm = sTerm.ToLower();
            IEnumerable<Product> products = await (from product in _context.Products join stock in _context.Stocks
                                                   on product.Id equals stock.ProductId into product_stock
                                                   from productWithStock in product_stock.DefaultIfEmpty()
                                                   where string.IsNullOrWhiteSpace(sTerm) || 
                                                   product.Name.ToLower().Contains(sTerm)
                                                   where string.IsNullOrWhiteSpace(category) ||
                                                   product.Category.ToLower().Contains(category)
                                                    select new Product
                                                   {
                                                       Id = product.Id,
                                                       Name = product.Name,
                                                       Brand = product.Brand,
                                                       Image = product.Image,
                                                       Price = product.Price,
                                                       Description = product.Description,
                                                       Category = product.Category,
                                                       Quantity = productWithStock == null? 0 : productWithStock.Quantity,
                                                   }).ToListAsync();

            return products;
        }
    }
}
