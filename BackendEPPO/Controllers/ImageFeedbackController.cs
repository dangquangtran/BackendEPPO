using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageFeedbackController : ControllerBase
    {
        private readonly IImageFeedbackService _service;

        public ImageFeedbackController(IImageFeedbackService IService)
        {
            _service = IService;
        }

     //   [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ImageFeedback.GetListImageFeedback_Endpoint)]
        public async Task<IActionResult> GetListImageFeedback(int page, int size)
        {
            var _imageFeedback = await _service.GetListImageFeedback(page, size);

            if (_imageFeedback == null || !_imageFeedback.Any())
            {
                return NotFound("No image feedback found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _imageFeedback
            });
        }

        // [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ImageFeedback.GetImageFeedbackByID)]
        public async Task<IActionResult> GetImageFeedbackByID(int id)
        {
            var _imageFeedback = await _service.GetImageFeedbackByID(id);

            if (_imageFeedback == null)
            {
                return NotFound($"Image Feedback with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _imageFeedback
            });
        }
    }
}
