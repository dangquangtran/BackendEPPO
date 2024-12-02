using BackendEPPO.Extenstion;
using DTOs.Address;
using DTOs.Notification;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Get list all Notification in database with the page and the size.
        /// </summary>
        /// <returns>Get list all notification in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Notification.GetListNotification_Endpoint)]
        public async Task<IActionResult> GetListNotification(int page, int size)
        {
            //var userIdClaim = User.FindFirst("userId")?.Value;
            //int userId = int.Parse(userIdClaim);

            //var noti = await _iNotificationService.GetListNotification(page, size, userId);

            //if (noti == null || !noti.Any())
            //{
            //    return NotFound("No notification found.");
            //}
            //return Ok(new
            //{
            //    StatusCode = 200,
            //    Message = "Request was successful",
            //    Data = noti
            //});
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return BadRequest("Invalid user ID.");
            }

            var groupedNotifications = await _iNotificationService.GetListNotification(page, size, userId);

            if (groupedNotifications == null || !groupedNotifications.Any())
            {
                return NotFound("No notifications found.");
            }

            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = groupedNotifications.Select(g => new
                {
                    Date = g.Key.ToString("yyyy-MM-dd"),
                    Notifications = g.Value.Select(n => new
                    {
                        n.Title,
                        n.Description,
                        n.CreatedDate
                    })
                })
            });
        }

        /// <summary>
        /// Get Notification by notification id.
        /// </summary>
        /// <returns> Get Notification by notification id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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

        /// <summary>
        /// Create Notification with all role.
        /// </summary>
        /// <returns> Create Notification with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Notification.CreateNotificationByID)]
        public async Task<IActionResult> CreateNotification([FromBody] CreateNotificationDTO notification)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _iNotificationService.CreateNotification(notification);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Notification created successfully",
                Data = notification
            });
        }

        /// <summary>
        /// Update Notification with all role.
        /// </summary>
        /// <returns> Update Notification with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Notification.UpdateNotificationByID)]
        public async Task<IActionResult> UpdateNotification(int id, [FromBody] UpdateNotificationDTO notification)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            notification.NotificationId = id;

            try
            {
                await _iNotificationService.UpdateNotification(notification);
                var updatedNoti = await _iNotificationService.GetNotificationByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Notification updated successfully.",
                    Data = updatedNoti
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Notification not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
