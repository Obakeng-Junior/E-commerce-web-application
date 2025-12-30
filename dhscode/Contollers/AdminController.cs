using DHSOnlineStore.Controllers;
using DHSOnlineStore.Data;
using DHSOnlineStore.DTOs;
using DHSOnlineStore.Models;
using DHSOnlineStore.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace CologneStore.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        private readonly IOrderRepository _orderRepository;

        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor contextAccessor, IOrderRepository orderRepository) : base(context, userManager, contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _orderRepository = orderRepository;
        }

        public async Task<IActionResult> AllOrders()
        {
            var orders = await _orderRepository.UserOrders(true);
            return View(orders);
        }

        public async Task<IActionResult> TogglePaymentStatus(int orderId)
        {
            try
            {
                await _orderRepository.TogglePaymentStatus(orderId);
            }
            catch (Exception)
            {

                throw;
            }

            return RedirectToAction(nameof(AllOrders));
        }

        public async Task<IActionResult> UpdateOrderStatus(int orderId)
        {
            var order = await _orderRepository.GetOrderById(orderId);
            if (order == null)
            {
                throw new InvalidOperationException($"Order with id: {orderId} was not found");
            }

            var orderStatusList = (await _orderRepository.GetOrderStatus()).Select(orderStatus =>
            {
                return new SelectListItem
                {
                    Value = orderStatus.Id.ToString(),
                    Text = orderStatus.StatusName,
                    Selected = order.OrderStatusId == orderStatus.Id
                };
            });

            var status = new OrderStatusDTO
            {
                OrderId = orderId,
                OrderStatusId = order.OrderStatusId,
                OrderStatusList = orderStatusList
            };

            return View(status);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(OrderStatusDTO status)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    status.OrderStatusList = (await _orderRepository.GetOrderStatus())
                        .Select(orderStatus =>
                        {
                            return new SelectListItem
                            {
                                Value = orderStatus.Id.ToString(),
                                Text = orderStatus.StatusName,
                                Selected = orderStatus.Id == status.OrderStatusId
                            };
                        });

                    return View(status);
                }

                await _orderRepository.ChangeOrderStatus(status);

            }
            catch (Exception)
            {

                TempData["errorMessage"] = "Update failed!";
                return View(status);
            }

            TempData["successMessage"] = "Updated Successfully!";
            return RedirectToAction("AllOrders");

            //return RedirectToAction(nameof(UpdateOrderStatus), new { orderId = data.OrderId });
        }
    }
}