﻿using DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService IService)
        {
            _orderService = IService;
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetAllOrders(int pageIndex, int pageSize)
        {
            try
            {
                var result = _orderService.GetAllOrders(pageIndex, pageSize);
                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn hàng nào.",
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = result
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

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var result = _orderService.GetOrderById(id);
                if (result == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Không tìm thấy đơn hàng với ID {id}.",
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = result
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

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderDTO createOrder)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                _orderService.CreateOrder(createOrder, userId);

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

        [Authorize]
        [HttpPut]
        public IActionResult UpdateOrder([FromBody] UpdateOrderDTO updateOrder)
        {
            try
            {
                _orderService.UpdateOrder(updateOrder);
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật đơn hàng thành công.",
                    Data = updateOrder
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

        [Authorize]
        [HttpGet("GetOrdersByUser")]
        public IActionResult GetOrdersByUserId(int pageIndex, int pageSize, int status)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                var result = _orderService.GetOrdersByUserId(userId, pageIndex, pageSize, status);
                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy đơn hàng nào của người dùng.",
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = result
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
