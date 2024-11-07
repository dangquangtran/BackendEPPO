using BackendEPPO.Extenstion;
using DTOs.ContractDetails;
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
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateFeedback(feedback, feedback.ImageFiles);

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
    }
}
