using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomParticipantController : ControllerBase
    {
        private readonly IRoomParticipantService _roomParticipantService;

        public RoomParticipantController(IRoomParticipantService IService)
        {
            _roomParticipantService = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.RoomParticipant.GetRoomParticipant_Endpoint)]
        public async Task<IActionResult> GetListRoomParticipant(int page, int size)
        {
            var room = await _roomParticipantService.GetListRoomParticipant(page, size);

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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.RoomParticipant.GetRoomParticipantByID)]
        public async Task<IActionResult> GetRoomParticipantByID(int id)
        {
            var room = await _roomParticipantService.GetRoomParticipantByID(id);

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
