using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.ContractDetails;
using DTOs.Error;
using DTOs.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _service;

        public FeedbacksController(IFeedbackService IService)
        {
            _service = IService;
        }

        /// <summary>
        /// Get list all Feedbacks in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Feedbacks in database with the page and the size.</returns>
        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Feedback.GetListFeedback_Endpoint)]
        public async Task<IActionResult> GetListFeedback(int page, int size)
        {
            var _feedback = await _service.GetListFeedback(page, size);

            if (_feedback == null || !_feedback.Any())
            {
                return NotFound("No feedback found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _feedback
            });
        }
        /// <summary>
        /// Function for mobile: Get list feedback of the plant.
        /// </summary>
        /// <returns>Get list all Feedbacks in database with the page and the size.</returns>
        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Feedback.GetListFeedbackByPlant)]
        public async Task<IActionResult> GetListFeedbackByPlant(int page, int size, int plantId)
        {
            var _feedback = await _service.GetListFeedbackByPlant(page, size, plantId);

            if (_feedback.Feedbacks == null || !_feedback.Feedbacks.Any())
            {
                return BadRequest(ModelState);
            }

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
                Data = new
                {
                    Feedbacks = _feedback.Feedbacks,
                    TotalRating = _feedback.TotalRating,
                    NumberOfFeedbacks = _feedback.NumberOfFeedbacks
                }
            });
        }



        /// <summary>
        /// Get feedback by feedback id.
        /// </summary>
        /// <returns>Get feedback by feedback id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Feedback.GetFeedbackByID)]
        public async Task<IActionResult> GetFeedbackByID(int id)
        {
            var _feedback = await _service.GetFeedbackByID(id);

            if (_feedback == null)
            {
                return NotFound($"Feedback with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _feedback
            });
        }

        /// <summary>
        /// Create feedback with all role.
        /// </summary>
        /// <returns>Create feedback with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Feedback.CreateFeedback)]
        public async Task<IActionResult> CreateFeedback([FromForm] CreateFeedbackDTO feedback)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateFeedback(feedback,userId, feedback.ImageFiles);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Feedback created successfully",
                Data = feedback
            });
        }

        /// <summary>
        /// Update feedback with all role.
        /// </summary>
        /// <returns>Update feedback with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Feedback.UpdateFeedbackID)]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackDTO feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            feedback.FeedbackId = id;

            try
            {
                await _service.UpdateFeedback(feedback);
                var updatedFeedback = await _service.GetFeedbackByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Feedback updated successfully.",
                    Data = updatedFeedback
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Feedback not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete feedback with all role.
        /// </summary>
        /// <returns>Delete feedback with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpDelete(ApiEndPointConstant.Feedback.DeleteFeedbackID)]
        public async Task<IActionResult> DeleteFeedback(int id, [FromBody] DeleteFeedbackDTO feedback)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            feedback.FeedbackId = id;

            try
            {
                await _service.DeleteFeedback(feedback);
                var updatedFeedback = await _service.GetFeedbackByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Feedback updated successfully.",
                    Data = updatedFeedback
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Feedback not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy danh sách feedback của các cây đã giao thành công và đã thanh toán.
        /// </summary>
        /// <param name="page">Trang hiện tại (bắt đầu từ 1).</param>
        /// <param name="size">Kích thước trang.</param>
        /// <returns>Get feedback with all role</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Feedback.GetListFeedbackOrderStatus)]
        //[HttpGet("delivered-plants-feedback")]
        public async Task<IActionResult> GetFeedbackByDeliveredPlants([FromQuery] int page, [FromQuery] int size , int TypeEcommerceId)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);

                // Gọi service để lấy danh sách feedback
                var feedbacks = await _service.GetFeedbackByDeliveredPlants(page, size, userId, TypeEcommerceId);

                // Kiểm tra nếu không có dữ liệu
                if (feedbacks == null || !feedbacks.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không tìm thấy feedback nào phù hợp."
                    });
                }

                // Trả về danh sách feedback
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách feedback thành công.",
                    Data = feedbacks
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng đã giao thành công để tạo feedback.
        /// </summary>
        /// <param name="userId">ID người dùng.</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="size">Số lượng đơn hàng trên mỗi trang.</param>
        /// <returns>Danh sách đơn hàng.</returns>
        ///     [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Feedback.GetListFeedbackOrderStatusDelivered)]
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        public async Task<IActionResult> GetDeliveredOrdersForFeedback( [FromQuery] int page, [FromQuery] int size)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);
                // Gọi service để lấy danh sách đơn hàng
                var orders = await _service.GetDeliveredOrdersForFeedback(userId, page, size);

                // Kiểm tra nếu không có dữ liệu
                if (orders == null || !orders.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không có đơn hàng nào phù hợp để tạo feedback."
                    });
                }

                // Trả về danh sách đơn hàng
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách đơn hàng thành công.",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Error = ex.Message
                });
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng đã giao thành công để tạo feedback.
        /// </summary>
        /// <param name="userId">ID người dùng.</param>
        /// <param name="page">Trang hiện tại.</param>
        /// <param name="size">Số lượng đơn hàng trên mỗi trang.</param>
        /// <returns>Danh sách đơn hàng.</returns>
        /// [Authorize(Roles = "admin, manager, staff, owner, customer")]

        [HttpGet(ApiEndPointConstant.Feedback.GetListFeedbackOrderDelivered)]
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        public async Task<IActionResult> GetDeliveredPlantsForFeedback([FromQuery] int page, [FromQuery] int size)
        {
            try
            {
                var userIdClaim = User.FindFirst("userId")?.Value;
                int userId = int.Parse(userIdClaim);
                // Gọi service để lấy danh sách đơn hàng
                var orders = await _service.GetDeliveredPlantsForFeedback(userId, page, size);

                // Kiểm tra nếu không có dữ liệu
                if (orders == null || !orders.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = "Không có đơn hàng nào phù hợp để tạo feedback."
                    });
                }

                // Trả về danh sách đơn hàng
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Lấy danh sách đơn hàng thành công.",
                    Data = orders
                });
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi khi xử lý yêu cầu.",
                    Error = ex.Message
                });
            }
        }
            /// <summary>
            /// Lấy danh sách đơn hàng đã giao thành công để tạo feedback.
            /// </summary>
            /// <param name="page">Trang hiện tại.</param>
            /// <param name="size">Số lượng đơn hàng trên mỗi trang.</param>
            /// <returns>Danh sách đơn hàng.</returns>
            /// [Authorize(Roles = "admin, manager, staff, owner, customer")]

            [HttpGet(ApiEndPointConstant.Feedback.GetListFeedbackOrderDeliveredRenting)]
            //[Authorize(Roles = "admin, manager, staff, owner, customer")]
            public async Task<IActionResult> GetDeliveredPlantsFeedbackRenting(int page, int size)
            {
                try
                {
                    // Gọi service để lấy danh sách cây
                    var plants = await _service.GetDeliveredPlantsFeedbackRenting(page, size);

                    // Kiểm tra nếu không có dữ liệu
                    if (plants == null || !plants.Any())
                    {
                        return NotFound(new
                        {
                            StatusCode = 404,
                            Message = "Không có cây nào phù hợp để tạo feedback."
                        });
                    }

                    // Trả về danh sách cây
                    return Ok(new
                    {
                        StatusCode = 200,
                        Message = "Lấy danh sách cây thành công.",
                        Data = plants
                    });
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi
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

