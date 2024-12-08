using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Category;
using DTOs.Error;
using DTOs.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

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

        /// <summary>
        /// Get list all room in database with the page and the size.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListRoom_Endpoint)]
        public async Task<IActionResult> GetListRooms(int page, int size)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            var room = await _roomService.GetListRooms(page, size, userId);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }
        /// <summary>
        /// Function for web: Get list all room in database with the page and the size.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListRoomManager_Endpoint)]
        public async Task<IActionResult> GetListRoomsManager(int page, int size)
        {
            var room = await _roomService.GetListRoomsManager(page, size);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }

        /// <summary>
        /// Get list all room by status in database with the page and the size.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListRoomStatus_Endpoint)]
        public async Task<IActionResult> GetListRoomsByStatus(int page, int size, int status)
        {
            var room = await _roomService.GetListRoomsByStatus(page, size, status);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }
        /// <summary>
        /// Get list room is aucting.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListRoomIsActive_Endpoint)]
        public async Task<IActionResult> GetListRoomsIsActive(int page, int size)
        {
            var room = await _roomService.GetListRoomsIsActive(page, size);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }
        /// <summary>
        /// Filter the list of rooms by price and the plant prices in descending with the page and the size..
        /// </summary>
        /// <returns>Filter list room by price.</returns>
        [HttpGet(ApiEndPointConstant.Room.FilterListRoomByPrice_Endpoint)]
        public async Task<IActionResult> FilterListRoomByPrice(int page, int size, double? minPrice = null, double? maxPrice = null, bool isDescending = false)
        {
            var room = await _roomService.FilterListRoomByPrice(page, size, minPrice, maxPrice, isDescending);

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

        /// <summary>
        /// Filter the list of rooms by date time have time near this play with the page and the size.
        /// </summary>
        /// <returns>Filter list room by date time.</returns>
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


        /// <summary>
        /// Search list room by date time with the page and the size.
        /// </summary>
        /// <returns>Search list room by date time.</returns>
        [HttpGet(ApiEndPointConstant.Room.SearchListRoomByDate_Endpoint)]
        public async Task<IActionResult> SearchListRoomByDate(int page, int size, string date)
        {

            var room = await _roomService.SearchListRoomByDate(page, size, date);

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

        /// <summary>
        /// Get room by ID of the room.
        /// </summary>
        /// <returns>Get room by ID of the room.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetRoomByID)]
        public async Task<IActionResult> GetRoomByID(int roomId)
        {
            try
            {
                int registeredCount = await _roomService.CountUserRegister(roomId);


                double totalSecoundOpening = await _roomService.CountTimeActive(roomId);
                if (totalSecoundOpening == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                double totalSecoundClosing = await _roomService.CountTimeClose(roomId);
                if (totalSecoundClosing == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                var room = await _roomService.GetRoomByID(roomId);

                if (room == null)
                {
                    return NotFound($"Room with ID {roomId} not found.");
                }
                //return Ok(new
                //{
                //    StatusCode = 200,
                //    Message = "Request was successful",
                //    Data = room
                //});
                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = new
                    {
                        Room = room,
                        RegisteredCount = registeredCount,
                        OpeningCoolDown = totalSecoundOpening,
                        ClosingCoolDown = totalSecoundClosing,

                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"{Error.ORDER_FOUND_ERROR}: {ex.Message}",
                    Data = (object)null
                });
            }

            
        }

        /// <summary>
        /// Create the room with role manager and staff.
        /// </summary>
        /// <returns> Create the room with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPost(ApiEndPointConstant.Room.CreateRoom)]
        public async Task<IActionResult> CreateRoom([FromBody] CreateRoomDTO room)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _roomService.CreateRoom(room);
                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Room created successfully",
                    Data = room
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Update the room with role manager and staff.
        /// </summary>
        /// <returns> Update the room with role manager and staff.</returns>
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
                var updatedRoom = await _roomService.GetRoomByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Room updated successfully.",
                    Data = updatedRoom
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

        /// <summary>
        /// Delete the room with role manager and staff to update status is zero.
        /// </summary>
        /// <returns>Delete the room with role manager and staff to update status is zero.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPut(ApiEndPointConstant.Room.UpdateStatusRoomByID)]
        public async Task<IActionResult> UpdateStatusRoom(int roomId, [FromBody] UpdateStatusRoomDTO room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
        
            try
            {
                await _roomService.UpdateStatusRoom(room, roomId);
                var updatedRoom = await _roomService.GetRoomByID(roomId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Delete room successfully.",
                    Data = updatedRoom
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

        /// <summary>
        /// Delete the room with role manager and staff to update status is zero.
        /// </summary>
        /// <returns>Delete the room with role manager and staff to update status is zero.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpDelete(ApiEndPointConstant.Room.DeleteRoomByID)]
        public async Task<IActionResult> DeleteRoom(int rooId, [FromBody] DeleteRoomDTO room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
          

            try
            {
                await _roomService.DeleteRoom(room, rooId);
                var updatedRoom = await _roomService.GetRoomByID(rooId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Delete room successfully.",
                    Data = updatedRoom
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


        /// <summary>
        /// Function Mobile: Get list room is active for register.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListRoomActive_Endpoint)]
        public async Task<IActionResult> GetListRoomActive(int page, int size)
        {
            var room = await _roomService.GetListRoomActive(page, size);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }


        /// <summary>
        /// Get list history room in database with the page and the size.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetListHistoryRoom_Endpoint)]
        public async Task<IActionResult> GetListHistoryRooms(int page, int size)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            var room = await _roomService.GetListHistoryRooms(userId,page, size);

            if (room == null || !room.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = room
            });
        }

        /// <summary>
        /// Get room by ID of the room.
        /// </summary>
        /// <returns>Get room by ID of the room.</returns>
        [HttpGet(ApiEndPointConstant.Room.GetRoomIDByCustomer)]
        public async Task<IActionResult> GetRoomIDByCustomer(int roomId)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            try
            {
                int registeredCount = await _roomService.CountUserRegister(roomId);


                double totalSecoundOpening = await _roomService.CountTimeActive(roomId);
                if (totalSecoundOpening == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                double totalSecoundClosing = await _roomService.CountTimeClose(roomId);
                if (totalSecoundClosing == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                var room = await _roomService.GetRoomIDByCustomer(roomId, userId);

                if (room == null)
                {
                    return NotFound($"Room with ID {roomId} not found.");
                }

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = new
                    {
                        Room = room,
                        RegisteredCount = registeredCount,
                        OpeningCoolDown = totalSecoundOpening,
                        ClosingCoolDown = totalSecoundClosing,

                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = $"{Error.ORDER_FOUND_ERROR}: {ex.Message}",
                    Data = (object)null
                });
            }


        }
    }
}
