﻿using BackendEPPO.Extenstion;
using DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersRentalController : ControllerBase
    {
        private readonly IOrderService _orderService;
        public OrdersRentalController(IOrderService IService)
        {
            _orderService = IService;
        }
        /// <summary>
        /// The create order with by more plants and more the owners in the Eppo
        /// </summary>
        /// <returns> The create order with by more plant and more the owner in the Eppo</returns>
        [Authorize(Roles = "admin, manager, customer")]
        [HttpPost(ApiEndPointConstant.OrderRental.CreateOrderRental)]
        public async Task<IActionResult> CreateOrderRentalAsync([FromBody] CreateOrderRentalDTO createOrderRental)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không thể xác thực người dùng.",
                        Data = (object)null
                    });
                }

                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "ID người dùng không hợp lệ.",
                        Data = (object)null
                    });
                }
                await _orderService.CreateOrderRentalAsync(createOrderRental, userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Đã tạo đơn hàng thành công.",
                    Data = createOrderRental
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Có lỗi xảy ra: " + ex.Message,
                    Data = (object)null
                });
            }
        }

    }
}
