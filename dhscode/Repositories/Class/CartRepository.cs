using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DHSOnlineStore.Repositories.Class
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public CartRepository(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public async Task<int> AddItem(int productId, int quantity)
        {
            string userId = GetUserId();
            //using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                    };
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();

                //cart detail section 
                var cartItem = _context.CartDetails.FirstOrDefault(c => c.CartId == cart.Id && c.ProductId == productId);
                if (cartItem == null)
                {
                    var product = _context.Products.Find(productId);
                    cartItem = new CartDetail
                    {
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    };
                    _context.CartDetails.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                //transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<int> IncreaseItem(int productId, int quantity)
        {
            string userId = GetUserId();
            //using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");
                var cart = await GetCart(userId);
                if (cart == null)
                {
                    cart = new Cart
                    {
                        UserId = userId,
                    };
                    _context.Carts.Add(cart);
                }
                _context.SaveChanges();

                //cart detail section 
                var cartItem = _context.CartDetails.FirstOrDefault(c => c.CartId == cart.Id && c.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                }
                else
                {
                    var product = _context.Products.Find(productId);
                    cartItem = new CartDetail
                    {
                        ProductId = productId,
                        CartId = cart.Id,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    };
                    _context.CartDetails.Add(cartItem);
                }

                await _context.SaveChangesAsync();
                //transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }
        public async Task<int> RemoveItem(int productId)
        {
            string userId = GetUserId();
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");
                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("Invalid cart");
                //cart detail section 
                var cartItem = _context.CartDetails.FirstOrDefault(c => c.CartId == cart.Id && c.ProductId == productId);
                if (cartItem == null)
                {
                    throw new Exception("No items in cart");
                }
                else if (cartItem.Quantity == 1)
                {
                    _context.CartDetails.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = cartItem.Quantity - 1;
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            var cartItemCount = await GetCartItemCount(userId);
            return cartItemCount;
        }

        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(x => x.UserId == userId);
            return cart;
        }

        public async Task<Cart> GetUserCart()
        {
            var userId = GetUserId();
            if (userId == null)
                throw new Exception("Invalid userId");
            var cart = await _context.Carts.Include(a => a.CartDetails).ThenInclude(a => a.Product)
                .ThenInclude(a => a.Stock).Where(a => a.UserId == userId).FirstOrDefaultAsync();

            return cart;
        }
        public async Task<bool> DoCheckOut(CheckoutDTO checkout)
        {
            //using var transaction = _context.Database.BeginTransaction();
            try
            {

                var userId = GetUserId();
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("User is not logged in");

                var cart = await GetCart(userId);
                if (cart == null)
                    throw new Exception("Invalid cart");

                var cartDetail = _context.CartDetails.Where(a => a.CartId == cart.Id).ToList();
                if (cartDetail.Count == 0)
                    throw new Exception("Cart is empty");

                var pendingRecord = _context.OrderStatus.FirstOrDefault
                    (a => a.StatusName == "Pending");
                if (pendingRecord == null)
                    throw new Exception("Order status does not have Pending status");

                var order = new Order
                {
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    UserName = checkout.Name,
                    Email = checkout.Email,
                    MobileNumber = checkout.MobileNumber,
                    Address = checkout.Address,
                    PaymentMethod = checkout.PaymentMethod,
                    IsPaid = false,
                    OrderStatusId = pendingRecord.Id
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                foreach (var item in cartDetail)
                {
                    var orderDetail = new OrderDetail
                    {
                        ProductId = item.ProductId,
                        OrderId = order.Id,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    };

                    _context.OrderDetails.Add(orderDetail);

                    //update stock
                    var stock = await _context.Stocks.FirstOrDefaultAsync(a => a.ProductId == item.ProductId);
                    if (stock == null)
                    {
                        throw new Exception("Stock is null");
                    }

                    if (item.Quantity > stock.Quantity)
                    {
                        throw new Exception($"Only {stock.Quantity} items are available in stock.");
                    }

                    //decrease stock quantity
                    stock.Quantity -= item.Quantity;
                }
                _context.SaveChanges();
                checkout.Message = "Your order was succesful! Keep an eye on your order status.";

                await CreateNotification(order.UserId, order.Id, checkout.Message);



                //remove cartdetails
                _context.CartDetails.RemoveRange(cartDetail);
                _context.SaveChanges();
                //transaction.Commit();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public async Task<int> GetCartItemCount(string userId = "")
        {
            if (!string.IsNullOrEmpty(userId))
            {
                userId = GetUserId();
            }

            var data = await (from cart in _context.Carts
                              join cartDetail in _context.CartDetails
                              on cart.Id equals cartDetail.CartId
                              select new { cartDetail.Id }).ToListAsync();

            return data.Count;
        }

        private string GetUserId()
        {
            var principal = _contextAccessor.HttpContext.User;
            string userId = _userManager.GetUserId(principal);

            return userId;
        }

        private async Task CreateNotification(string userId, int orderId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                OrderId = orderId,
                Message = message,
                IsRead = false,
                CreatedDate = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}
