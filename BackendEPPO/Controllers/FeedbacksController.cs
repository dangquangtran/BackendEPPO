using BackendEPPO.Extenstion;
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

     //   [Authorize(Roles = "admin, manager, staff, owner, customer")]
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

   //     [Authorize(Roles = "admin, manager, staff, owner, customer")]
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
    }
}
