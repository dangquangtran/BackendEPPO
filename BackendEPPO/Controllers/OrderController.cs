using DTOs.Order;
using GoogleApi.Entities.Search.Video.Common;
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
        [HttpGet("GetOrdersBuyByUser")]
        public IActionResult GetOrdersBuyByUser(int pageIndex, int pageSize, int status)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                var result = _orderService.GetOrdersBuyByUserId(userId, pageIndex, pageSize, status);
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

        [Authorize]
        [HttpGet("GetOrdersRentalByUser")]
        public IActionResult GetOrdersRentalByUserId(int pageIndex, int pageSize, int status)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                var result = _orderService.GetOrdersRentalByUserId(userId, pageIndex, pageSize, status);
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

        [Authorize]
        [HttpPost("CreateOrderRental")]
        public IActionResult CreateOrderRental([FromBody] CreateOrderRentalDTO createOrderRental)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                var order = _orderService.CreateRentalOrder(createOrderRental, userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Đã tạo đơn hàng thành công.",
                    Data = order
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
        [HttpPut("UpdatePaymentOrderRental")]
        public IActionResult UpdatePaymentOrderRental(int orderId, int contractId, int paymentId)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                _orderService.UpdatePaymentOrderRental(orderId, contractId, userId, paymentId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật trạng thái thanh toán đơn hàng thành công.",
                    Data = (object)null
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
        [HttpPut("CancelOrder/{orderId}")]
        public IActionResult CancelOrder(int orderId)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                _orderService.CancelOrder(orderId, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã hủy đơn hàng thành công.",
                    Data = (object)null
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
        [HttpPut("UpdatePreparedOrderSuccess/{orderId}")]
        public async Task<IActionResult> UpdatePreparedOrderSuccess(int orderId)
        {
            try
            {
                // Lấy userId từ JWT claims
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không có quyền truy cập.",
                        Data = (object)null
                    });
                }

                int userId = int.Parse(userIdClaim);

                // Gọi hàm dịch vụ để cập nhật đơn hàng
                await _orderService.UpdatePreparedOrderSuccess(orderId, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật trạng thái giao hàng thành công.",
                    Data = (object)null
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
        [HttpPut("UpdateDeliverOrderSuccess/{orderId}")]
        public async Task<IActionResult> UpdateDeliverOrderSuccess(int orderId, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                // Lấy userId từ JWT claims
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không có quyền truy cập.",
                        Data = (object)null
                    });
                }

                int userId = int.Parse(userIdClaim);

                // Gọi hàm dịch vụ để cập nhật đơn hàng
                await _orderService.UpdateDeliverOrderSuccess(orderId, imageFiles, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật trạng thái giao hàng thành công.",
                    Data = (object)null
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
        [HttpPut("UpdateDeliverOrderFail/{orderId}")]
        public async Task<IActionResult> UpdateDeliverOrderFail(int orderId, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                // Lấy userId từ JWT claims
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không có quyền truy cập.",
                        Data = (object)null
                    });
                }

                int userId = int.Parse(userIdClaim);

                // Gọi hàm dịch vụ để cập nhật đơn hàng
                await _orderService.UpdateDeliverOrderFail(orderId, imageFiles, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật trạng thái giao hàng thành công.",
                    Data = (object)null
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
        [HttpPut("UpdateReturnOrderSuccess/{orderId}")]
        public async Task<IActionResult> UpdateReturnOrderSuccess(int orderId, [FromForm] List<IFormFile> imageFiles)
        {
            try
            {
                // Lấy userId từ JWT claims
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không có quyền truy cập.",
                        Data = (object)null
                    });
                }

                int userId = int.Parse(userIdClaim);

                // Gọi hàm dịch vụ để cập nhật đơn hàng
                await _orderService.UpdateReturnOrderSuccess(orderId, imageFiles, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật trạng thái giao hàng thành công.",
                    Data = (object)null
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
        [HttpPut("UpdateOrderStatus")]
        public IActionResult UpdateOrderStatus([FromQuery] int orderId, int newStatus)
        {
            try
            {
                // Lấy userId từ JWT claims
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new
                    {
                        StatusCode = 401,
                        Message = "Không có quyền truy cập.",
                        Data = (object)null
                    });
                }

                int userId = int.Parse(userIdClaim);

                // Gọi phương thức UpdateOrderStatus trong Service
                _orderService.UpdateOrderStatus(orderId, newStatus, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã cập nhật trạng thái đơn hàng thành công.",
                    Data = (object)null
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

        [HttpGet("GetOrdersByOwner")]
        [Authorize]
        public IActionResult GetOrdersByOwner([FromQuery] int pageIndex, int pageSize)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            try
            {
                // Gọi đến hàm service để lấy dữ liệu
                var orders = _orderService.GetOrdersByOwner(userId, pageIndex, pageSize);

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

        [Authorize]
        [HttpGet("GetOrdersByTypeEcommerceId")]
        public IActionResult GetOrdersByTypeEcommerceId(
           [FromQuery] int typeEcommerceId,
            DateTime? startDate,
            DateTime? endDate,
            int pageIndex,
            int pageSize)
        {
            try
            {
                // Gọi phương thức trong service để lấy danh sách đơn hàng
                var result = _orderService.GetOrdersByTypeEcommerceId(typeEcommerceId, startDate, endDate, pageIndex, pageSize);

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
        [HttpGet("GetOrdersAuctionByUser")]
        public IActionResult GetOrdersAuctionByUserId(int pageIndex, int pageSize, int status)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                var result = _orderService.GetOrdersAuctionByUserId(userId, pageIndex, pageSize, status);
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
