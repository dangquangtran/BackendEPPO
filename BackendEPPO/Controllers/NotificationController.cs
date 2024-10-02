using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _iNotificationService;

        public NotificationController(INotificationService IService)
        {
            _iNotificationService = IService;
        }

        [HttpGet(ApiEndPointConstant.Notification.GetListNotification_Endpoint)]
        public async Task<IActionResult> GetListNotification(int page, int size)
        {
            var noti = await _iNotificationService.GetListNotification(page, size);

            if (noti == null || !noti.Any())
            {
                return NotFound("No notification found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = noti
            });
        }

        [HttpGet(ApiEndPointConstant.Notification.GetNotificationByID)]
        public async Task<IActionResult> GetNotificationByID(int id)
        {
            var noti = await _iNotificationService.GetNotificationByID(id);

            if (noti == null)
            {
                return NotFound($"Notification with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = noti
            });
        }



    }
}
