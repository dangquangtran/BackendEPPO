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
            return Ok(_orderService.GetAllOrders(pageIndex, pageSize));
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(int id)
        {
            return Ok(_orderService.GetOrderById(id));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreateOrder([FromBody] CreateOrderDTO createOrder)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            _orderService.CreateOrder(createOrder, userId);
            return Ok("Đã tạo thành công");
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdateOrder([FromBody] UpdateOrderDTO updateOrder)
        {
            _orderService.UpdateOrder(updateOrder);
            return Ok("Đã cập nhật thành công");
        }
        [Authorize]
        [HttpGet("GetOrdersByUser")]
        public IActionResult GetOrdersByUserId(int pageIndex, int pageSize, int status)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            return Ok(_orderService.GetOrdersByUserId(userId,pageIndex, pageSize, status));
        }

    }
}
