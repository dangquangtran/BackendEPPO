using BackendEPPO.Extenstion;
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
    }
}
