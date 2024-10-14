using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubFeedbackController : ControllerBase
    {
        private readonly ISubFeedbackService _service;

        public SubFeedbackController(ISubFeedbackService IService)
        {
            _service = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.SubFeedback.GetListSubFeedback_Endpoint)]
        public async Task<IActionResult> GetListSubFeedback(int page, int size)
        {
            var _subFeedback = await _service.GetListSubFeedback(page, size);

            if (_subFeedback == null || !_subFeedback.Any())
            {
                return NotFound("No SubFeedback found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _subFeedback
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.SubFeedback.GetSubFeedbackByID)]
        public async Task<IActionResult> GetSubFeedbackByID(int id)
        {
            var _subFeedback = await _service.GetSubFeedbackByID(id);

            if (_subFeedback == null)
            {
                return NotFound($"SubFeedback with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _subFeedback
            });
        }
    }
}
