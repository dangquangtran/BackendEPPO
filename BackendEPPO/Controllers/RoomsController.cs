using BackendEPPO.Extenstion;
using DTOs.Category;
using DTOs.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService IService)
        {
            _roomService = IService;
        }


        [HttpGet(ApiEndPointConstant.Room.GetListRoom_Endpoint)]
        public async Task<IActionResult> GetListRooms(int page, int size)
        {
            var room = await _roomService.GetListRooms(page, size);

            if (room == null || !room.Any())
            {
                return NotFound("No room found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = room
            });
        }
        [HttpGet(ApiEndPointConstant.Room.GetListRoomByDateNow_Endpoint)]
        public async Task<IActionResult> GetListRoomsByDateNow(int page, int size)
        {
          
            var room = await _roomService.GetListRoomsByDateNow(page, size);

            if (room == null || !room.Any())
            {
                return NotFound("No room found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = room
            });
        }

        [HttpGet(ApiEndPointConstant.Room.GetRoomByID)]
        public async Task<IActionResult> GetRoomByID(int id)
        {
            var room = await _roomService.GetRoomByID(id);

            if (room == null)
            {
                return NotFound($"Room with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = room
            });
        }
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPost(ApiEndPointConstant.Room.CreateRoom)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDTO room)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _roomService.CreateRoom(room);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Room created successfully",
                Data = room
            });
        }
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPut(ApiEndPointConstant.Room.UpdateRoomByID)]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] UpdateRoomDTO room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            room.RoomId= id;

            try
            {
                await _roomService.UpdateRoom(room);
                var updatedcRoom = await _roomService.GetRoomByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Room updated successfully.",
                    Data = updatedcRoom
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Room not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
