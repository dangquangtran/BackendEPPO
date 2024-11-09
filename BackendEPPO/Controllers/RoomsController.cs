﻿using BackendEPPO.Extenstion;
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

        /// <summary>
        /// Get list all room in database with the page and the size.
        /// </summary>
        /// <returns>Get list all room in database.</returns>
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
        [HttpDelete(ApiEndPointConstant.Room.DeleteRoomByID)]
        public async Task<IActionResult> DeleteRoom(int id, [FromBody] DeleteRoomDTO room)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            room.RoomId = id;

            try
            {
                await _roomService.DeleteRoom(room);
                var updatedRoom = await _roomService.GetRoomByID(id);

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
    }
}
