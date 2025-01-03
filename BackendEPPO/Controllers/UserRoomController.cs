﻿using BackendEPPO.Extenstion;
using DTOs.Error;
using DTOs.Room;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Implements;
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
        /// Function for mobile: Get list all room register successful by token .
        /// </summary>
        /// <returns>Function for mobile: Get list all room register successful by token.</returns>
        [Authorize(Roles = "customer")]
        [HttpGet(ApiEndPointConstant.UserRoom.GetListUserRoomByToken_Endpoint)]
        public async Task<IActionResult> GetListUserRoomWithUserToken(int page, int size)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            var room = await _service.GetListUserRoomWithUserToken(page, size, userId);

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
        /// Get user room  to register by  room id.
        /// </summary>
        /// <param name="roomId">Room ID(e.g., "roomID = 10")</param>
        /// <returns>Get user room by user room id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserRoom.GetUserRoomByID)]
        public async Task<IActionResult> GetUserRoomByRoomID(int roomId)
        {
            try
            {
                double totalSecoundOpening = await _service.CountTimeActive(roomId);
                if (totalSecoundOpening == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                double totalSecoundClosing = await _service.CountTimeClose(roomId);
                if (totalSecoundClosing == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                var room = await _service.GetUserRoomByRoomID(roomId);
                if (room == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

     
                int registeredCount = await _service.CountUserRegister(roomId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = new
                    {
                        Room = room,
                        RegisteredCount =  registeredCount,
                        OpeningCoolDown =  totalSecoundOpening,
                        ClosingCoolDown =  totalSecoundClosing,

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
        /// Get user room  details to register by user room id.
        /// </summary>
        /// <param name="userRoomId">Room ID(e.g., "userRoomId = 10")</param>
        /// <returns>Get user room by user room id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserRoom.GetUserRoomByRoomID)]
        public async Task<IActionResult> GetUserRoomByID(int userRoomId)
        {
            try
            {
                var room = await _service.GetUserRoomByID(userRoomId);
                if (room == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }


                double totalSecoundOpening = await _service.CountTimeActive(userRoomId);
                if (totalSecoundOpening == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }

                double totalSecoundClosing = await _service.CountTimeClose(userRoomId);
                if (totalSecoundClosing == null)
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = Error.NO_DATA_FOUND,
                        Data = (object)null
                    });
                }



                int registeredCount = await _service.CountUserRegister(userRoomId);

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

            try
            {
                await _service.CreateUserRoom(userRoom, userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = userRoom
                });
            }
            catch (Exception ex)
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Message = ex.Message
                });
            }
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
                return BadRequest(new { Message = Error.BAD_REQUEST });
            }
            userRoom.UserRoomId = id;

            try
            {
                await _service.UpdateUserRoom(userRoom);
                var updatedcRoom = await _service.GetUserRoomByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedcRoom
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = Error.ERROR_500, error = ex.Message });
            }
        }


        /// <summary>
        /// Delete user room with role manager, role staff and role customer
        /// </summary>
        /// <returns>Delete user room with role manager, role staff and role customer.</returns>
        [Authorize(Roles = "admin, manager, staff, customer")]
        [HttpDelete(ApiEndPointConstant.UserRoom.DelteUserRoomByID)]
        public async Task<IActionResult> DeleteUserRoom(int userRoomId, [FromBody] DeleteUserRoomDTO userRoom)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.BAD_REQUEST });
            }


            try
            {
                await _service.DeleteUserRoom(userRoom , userRoomId);
                var updatedcRoom = await _service.GetUserRoomByID(userRoomId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedcRoom
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = Error.ERROR_500, error = ex.Message });
            }
        }


    }
}
