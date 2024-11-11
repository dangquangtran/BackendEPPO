using DTOs.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            try
            {
                var result = _orderService.GetOrderById(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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

                return Ok(new { message = "Đã tạo thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        [Authorize]
        [HttpPut]
        public IActionResult UpdateOrder([FromBody] UpdateOrderDTO updateOrder)
        {
            try
            {
                _orderService.UpdateOrder(updateOrder);
                return Ok(new { message = "Đã cập nhật thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
