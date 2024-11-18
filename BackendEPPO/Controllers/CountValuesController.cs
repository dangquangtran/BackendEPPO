using BackendEPPO.Extenstion;
using DTOs.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountValuesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IUserRoomService _userRoomService;
        public CountValuesController(IOrderService orderService, IUserService userService, IUserRoomService userRoomService)
        {
            _orderService = orderService;
            _userService = userService;
            _userRoomService = userRoomService;
        }
        /// <summary>
        /// Function for web: Count the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountOrder_Endpoint)]
        public IActionResult CountOrderByStatus(int userId, int status)
        {
            try
            {
                int result = _orderService.CountOrderByStatus(userId, status).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.ORDER_FOUND_ERROR,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = $"Số đơn hàng là: {result}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.ORDER_FOUND_ERROR + ex.Message,
                    Data = (object)null
                });
            }
        }

        /// <summary>
        /// Function for mobile: Count the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "owner, customer")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderByToken_Endpoint)]
        public IActionResult CountOrderStatusByToken(int status)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            try
            {
                int result = _orderService.CountOrderByStatus(userId, status).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.ORDER_FOUND_ERROR,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = $"Số đơn hàng là: {result}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.ORDER_FOUND_ERROR + ex.Message,
                    Data = (object)null
                });
            }
        }

        /// <summary>
        /// Function for web: Count account by status.
        /// </summary>
        /// <returns> Function for web: Count account by status.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountAccountByStatus_Endpoint)]
        public IActionResult CountAccountByStatus(int status)
        {
            try
            {
                int result = _userService.CountAccountByStatus(status).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.ORDER_FOUND_ERROR,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = $"Số tài khoảng là: {result}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.ORDER_FOUND_ERROR + ex.Message,
                    Data = (object)null
                });
            }
        }
        /// <summary>
        /// Function for web: Count revenue the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderPriceRevenue_Endpoint)]
        public IActionResult CountOrderPriceCountOrderPrice(int status, int? month = null, int? year = null )
        {
            try
            {
                double result = _orderService.CountOrderPrice(status, month, year).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.ORDER_FOUND_ERROR,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = $"Doanh thu của bạn là: {result}"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.ORDER_FOUND_ERROR + ex.Message,
                    Data = (object)null
                });
            }
        }
    }
}
