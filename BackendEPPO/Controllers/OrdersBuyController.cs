﻿using BackendEPPO.Extenstion;
using DTOs.Order;
using DTOs.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersBuyController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersBuyController(IOrderService IService)
        {
            _orderService = IService;
        }

        /// <summary>
        /// The create order with by more plants and more the owners in the Eppo
        /// </summary>
        /// <returns> The create order with by more plant and more the owner in the Eppo</returns>
        [Authorize(Roles = "admin, manager, customer")]
        [HttpPost(ApiEndPointConstant.OrderBy.CreateOrderBy)]
        public async Task<IActionResult> CreateOrderBuyAsync([FromBody] CreateOrderDTO createOrder)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Người dùng chưa được xác thực."
                    });
                }
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return BadRequest(new
                    {
                        StatusCode = 400,
                        Message = "Thông tin người dùng không hợp lệ."
                    });
                }

                await _orderService.CreateOrderBuyAsync(createOrder, userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Đã tạo đơn hàng thành công.",
                    Data = createOrder
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


        /// <summary>
        /// The get order with by the owners with status in the Eppo
        /// </summary>
        /// <returns> The create order with by more plant and more the owner in the Eppo</returns>
        [HttpGet("GetOrdersByOwner")]
        [HttpGet(ApiEndPointConstant.OrderBy.CreateOrderBy)]
        [Authorize]
        public IActionResult GetOrdersByOwnerByStatus([FromQuery] int pageIndex, int pageSize, int? status, bool? isReturnSoon)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            try
            {
                // Gọi đến hàm service để lấy dữ liệu
                var orders = _orderService.GetOrdersByOwnerByStatus(userId, pageIndex, pageSize, status, isReturnSoon);

                if (!orders.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Không tìm thấy đơn hàng nào.",
                        Data = (object)null
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo
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
