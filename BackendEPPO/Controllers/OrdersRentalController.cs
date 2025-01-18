using BackendEPPO.Extenstion;
using DTOs.Order;
using DTOs.Plant;
using GoogleApi.Entities.Search.Video.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersRentalController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPlantService _plantService;

        private readonly IContractService _contractService;
        public OrdersRentalController(IOrderService IService, IPlantService plantService, IContractService contractService)
        {
            _orderService = IService;
            _plantService = plantService;
            _contractService = contractService;
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
                var order = await _orderService.CreateOrderRentalAsync(createOrderRental, userId);

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
        /// <summary>
        /// Get Order Rental by Id
        /// </summary>
        /// <returns> The create order with by more plant and more the owner in the Eppo</returns>
        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.OrderRental.ViewReturnOrderRental)]
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        public async Task<IActionResult> GetOrderByID(int OrderId)
        {
            try
            {
                // Fetch the order using the service
                var order = await _orderService.GetOrderRentalByID(OrderId);

                if (order == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Order with ID {OrderId} not found."
                    });
                }
                var plantDetails = order.OrderDetails.FirstOrDefault()?.PlantId;
                PlantVM plant = null;
                if (plantDetails != null)
                {
                    plant =  _plantService.GetPlantById((int)plantDetails);
                }

                var contract = await _contractService.GetContractByOrderId(OrderId);


                var deposit = order.OrderDetails.FirstOrDefault()?.Deposit;
                var numberMonth = order.OrderDetails.FirstOrDefault()?.NumberMonth;
                //var numberDate = DateTime.Now - order.OrderDetails.FirstOrDefault()?.RentalStartDate;

                var rentalEndDate = order.OrderDetails.FirstOrDefault()?.RentalStartDate;

              
                    var numberDate = (DateTime.Now - rentalEndDate.Value ).TotalDays;

                    // Làm tròn số ngày
                    int roundedDays = (int)Math.Round(numberDate) + 1;
                



                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Order retrieved successfully.",
                    Data = new
                    {
                        Order = order,
                        Plant = plant ,
                        Contract = contract,
                        Deposit = deposit,
                        NumberMonth = numberMonth,
                        NumberDateRental = roundedDays,

                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "An error occurred: " + ex.Message,
                    Data = (object)null
                });
            }

        }

        /// <summary>
        /// Customer confirm submit Return order rental soon deadline
        /// </summary>
        /// <returns> Customer confirm submit Return order rental soon deadlinereturns>
        [HttpPut(ApiEndPointConstant.OrderRental.UpdateReturnOrderRental)]
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        public async Task<IActionResult> UpdateOrdersReturnAsync([FromQuery] int orderId)
        {
            try
            {
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

                var updatedOrder = await _orderService.UpdateOrdersReturnAsync(orderId, userId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Đã xác nhận trả cây thành công",
                    Data = updatedOrder
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
