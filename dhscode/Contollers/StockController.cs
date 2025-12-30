using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CologneStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StockController : BaseController
    {
        private readonly IStockRepository _stockRepository;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public StockController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IStockRepository stockRepository) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _stockRepository = stockRepository;
        }

        public async Task<IActionResult> Index(string sTerm = "")
        {
            var stock = await _stockRepository.GetStock(sTerm);
            return View(stock);
        }

        public async Task<IActionResult> ManageStock(int productId)
        {
            var existingStock = await _stockRepository.GetStockByProductId(productId);
            var stock = new StockDTO
            {
                ProductId = productId,
                Quantity = existingStock != null ? existingStock.Quantity : 0,
            };

            return View(stock);
        }

        [HttpPost]
        public async Task<IActionResult> ManageStock(StockDTO stock)
        {
            if (!ModelState.IsValid)
            {
                return View(stock);
            }

            try
            {
                await _stockRepository.ManageStock(stock);
                TempData["successMessage"] = "Stock is updated successfully!";
            }
            catch (Exception)
            {

                TempData["errorMessage"] = "Stock is not updated.";
            }

            return RedirectToAction("Index");
        }
    }
}