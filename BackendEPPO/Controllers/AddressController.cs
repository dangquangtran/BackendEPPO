﻿using BackendEPPO.Extenstion;
using DTOs.Address;
using DTOs.Rank;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Implements;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _IService;

        public AddressController(IAddressService _addressService)
        {
            _IService = _addressService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetListAddress_Endpoint)]
        public async Task<IActionResult> GetListUsers(int page, int size)
        {
            var users = await _IService.GetLisAddress(page, size);

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = users
            });
        }
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetAddressByID)]
        public async Task<IActionResult> GetUsersByID(int id)
        {
            var users = await _IService.GetAddressByID(id);

            if (users == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = users
            });
        }
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Address.CreateAddress)]
        public async Task<IActionResult> CreateAddress([FromBody] RequestAddress address)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _IService.CreateAddress(address);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Address created successfully",
                Data = address
            });
        }
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Address.UpdateAddress)]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] UpdateAddressDTO address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            address.AddressId = id;

            try
            {
                await _IService.UpdateAddress(address);
                var updatedRank = await _IService.GetAddressByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Address updated successfully.",
                    Data = updatedRank
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Address not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
