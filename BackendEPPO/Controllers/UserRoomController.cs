using BackendEPPO.Extenstion;
using DTOs.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoomController : ControllerBase
    {
        private readonly IUserRoomService _service;

        public UserRoomController(IUserRoomService IService)
        {
            _service = IService;
        }

        /// <summary>
        /// Get list all user room in database with the page and the size.
        /// </summary>
        /// <returns>Get list all user room in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserRoom.GetListUserRoom_Endpoint)]
        public async Task<IActionResult> GetListUserRoom(int page, int size)
        {
            var room = await _service.GetListUserRoom(page, size);

            if (room == null || !room.Any())
            {
                return NotFound("No user room found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = room
            });
        }

        /// <summary>
        /// Get user room by user room id.
        /// </summary>
        /// <returns>Get user room by user room id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserRoom.GetUserRoomByID)]
        public async Task<IActionResult> GetUserRoomByID(int id)
        {
            var room = await _service.GetUserRoomByID(id);

            if (room == null)
            {
                return NotFound($"User Room with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = room
            });
        }


        /// <summary>
        /// Register to bid with role manager, role staff and role customer
        /// </summary>
        /// <returns>Register to bid with role manager, role staff and role customer.</returns>
        [Authorize(Roles = "admin, manager, staff, customer")]
        [HttpPost(ApiEndPointConstant.UserRoom.CreateUserRoom)]
        public async Task<IActionResult> CreateUserRoom([FromBody] CreateUserRoomDTO userRoom)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateUserRoom(userRoom, userId);

            return Ok(new
            {
                StatusCode = 201,
                Message = "User room created successfully",
                Data = userRoom
            });
        }

        /// <summary>
        /// Update user room with role manager, role staff and role customer
        /// </summary>
        /// <returns>Update user room with role manager, role staff and role customer.</returns>
        [Authorize(Roles = "admin, manager, staff, customer")]
        [HttpPut(ApiEndPointConstant.UserRoom.UpdateUserRoomByID)]
        public async Task<IActionResult> UpdateUserRoom(int id, [FromBody] UpdateUserRoomDTO userRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            userRoom.UserRoomId = id;

            try
            {
                await _service.UpdateUserRoom(userRoom);
                var updatedcRoom = await _service.GetUserRoomByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "User room updated successfully.",
                    Data = updatedcRoom
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User room not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
