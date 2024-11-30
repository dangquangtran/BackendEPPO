using BackendEPPO.Extenstion;
using DTOs.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountValuesController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IUserRoomService _userRoomService;
        private readonly IPlantService _plantService;

        public CountValuesController(IOrderService orderService, IUserService userService, IUserRoomService userRoomService, IPlantService plantService)
        {
            _orderService = orderService;
            _userService = userService;
            _userRoomService = userRoomService;
            _plantService = plantService;
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
        /// Function for web: Count account by status.
        /// </summary>
        /// <returns> Function for web: Count account by status.</returns>
        //[Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountCustomerByStatus_Endpoint)]
        public IActionResult CountAccountCustomer()
        {
            try
            {
                int result = _userService.CountAccountCustomer(1).Result;

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
                    Data =  result
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

        /// <summary>
        /// Function for web: Count revenue the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderPriceRevenue12M_Endpoint)]
        public async Task<IActionResult> CountOrderPriceForYear(int status, int year)
        {
            try
            {
                var revenueData = await _orderService.CountOrderPriceForYear(status, year);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = revenueData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Đã xảy ra lỗi: " + ex.Message,
                    Data = (object)null
                });
            }
        }

        /// <summary>
        /// Function for web: Count revenue the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderTypeEcommerceId_Endpoint)]
        public async Task<IActionResult> CountOrderPriceByTypeEcom(int status, int year)
        {
            try
            {
                // Lấy dữ liệu doanh thu theo từng loại
                var sellRevenue = await _orderService.CountOrderPriceByTypeEcom(status, year, 1);
                var  rentRevenue = await _orderService.CountOrderPriceByTypeEcom(status, year, 2);
                var  auctionRevenue = await _orderService.CountOrderPriceByTypeEcom(status, year, 3);

                // Tính tổng doanh thu từng loại
                var totalAuction = auctionRevenue.Sum();
                var totalSell = sellRevenue.Sum();
                var totalRent = rentRevenue.Sum();

                // Tính tổng doanh thu
                var totalRevenue = totalAuction + totalSell + totalRent;

                // Tính phần trăm
                var auctionPercentage = totalRevenue > 0 ? (totalAuction * 100.0) / totalRevenue : 0;
                var sellPercentage = totalRevenue > 0 ? (totalSell * 100.0) / totalRevenue : 0;
                var rentPercentage = totalRevenue > 0 ? (totalRent * 100.0) / totalRevenue : 0;

                // Trả về dữ liệu
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = new
                    {
                        Percentages = new
                        {
                            Auction = auctionPercentage,
                            Sell = sellPercentage,
                            Rent = rentPercentage
                        },
                        Revenues = new
                        {
                            Auction = totalAuction,
                            Sell = totalSell,
                            Rent = totalRent
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Đã xảy ra lỗi: " + ex.Message,
                    Data = (object)null
                });
            }
        }




        /// <summary>
        /// Function for mobile: Count account register by status.
        /// </summary>
        /// <returns> Function for web: Count account by status.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Count.CountShipByPlant)]
        public IActionResult CountShipByPlant(int plantId)
        {
            try
            {
                int result = _plantService.CountShipByPlant(plantId).Result;

                if (result == 0)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.BAD_REQUEST,
                        Data = 0
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = result
            });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = Error.NO_DATA_FOUND + ex.Message,
                    Data = (object)null
                });
            }
        }
        /// <summary>
        /// Function for web: Count the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderStatus_Endpoint)]
        public IActionResult CountOrderByStatus()
        {
            try
            {
                int result = _orderService.CountOrderByStatus( 0).Result;

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
                    Data = result
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
        /// Function for web: Count the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderTotalRevenue_Endpoint)]
        public IActionResult CountOrderPrice()
        {
            try
            {
                double result = _orderService.CountOrderPrice(0).Result;

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
                    Data = result
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
        /// Function for web: Count the order by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Count.CountOrderTodayRevenue_Endpoint)]
        public IActionResult CountOrderPriceDateNow()
        {
            try
            {
                double result = _orderService.CountOrderPriceDateNow(0).Result;

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
                    Data = result
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
        /// Function for web: Count the top customer by status.
        /// </summary>
        /// <returns>Function off manager: Count the order by status.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.User.GetListTopCustomers)]
        public async Task<IActionResult> GetTopCustomersByWalletBalance(int page, int size)
        {
            try
            {
                var customers = await _userService.GetTopCustomersByWalletBalance(page, size);

                if (customers == null || !customers.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không có khách hàng nào phù hợp."
                    });
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách khách hàng thành công.",
                    Data = customers
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Error = ex.Message
                });
            }
        }


    }
}
