using BackendEPPO.Extenstion;
using DTOs.Address;
using DTOs.Error;
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

        /// <summary>
        /// Get list all the address in database.
        /// </summary>
        /// <returns>Get list all the address in database.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetListAddress_Endpoint)]
        public async Task<IActionResult> GetListAddress(int page, int size)
        {
            var address = await _IService.GetLisAddress(page, size);

            if (address == null || !address.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = address
            });
        }

        /// <summary>
        /// Get list the address of by user with userID.
        /// </summary>
        /// <returns>Get list the address of by user with userID.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetListAddressByUserID_Endpoint)]
        public async Task<IActionResult> GetListAddressByUserID(int userID)
        {
            var address = await _IService.GetLisAddressByUserID(userID);

            if (address == null || !address.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = address
            });
        }

        /// <summary>
        /// Get the address by addressId.
        /// </summary>
        /// <returns>Get the address by addressId.</returns>
     //   [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetAddressByID)]
        public async Task<IActionResult> GetAddressByID(int addressId)
        {
            var address = await _IService.GetAddressByID(addressId);

            if (address == null)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = address
            });
        }

        /// <summary>
        /// Get the address by addressId for role customer and owner.
        /// </summary>
        /// <returns>Get the address by addressId.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Address.GetListAddressByToken_Endpoint)]
        public async Task<IActionResult> GetListAddressByToken()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            var users = await _IService.GetLisAddressByUserID(userId);

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = users
            });
        }

        /// <summary>
        /// Create the address with all role.
        /// </summary>
        /// <returns>Create the address with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Address.CreateAddress)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateAddressDTO address)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _IService.CreateAddress(address, userId);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
                Data = address
            });
        }

        /// <summary>
        /// Update the address with all role.
        /// </summary>
        /// <returns>Update the address with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Address.UpdateAddress)]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromForm] UpdateAddressDTO address)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            try
            {
                await _IService.UpdateAddress(address , addressId, userId);
                var updatedAddress = await _IService.GetAddressByID(addressId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedAddress
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
        /// Delete the address with all role.
        /// </summary>
        /// <returns>Delete the address with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpDelete(ApiEndPointConstant.Address.DeleteAddress)]
        public async Task<IActionResult> DeleteAddress(int addressId, [FromBody] DeleteAddressDTO address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.BAD_REQUEST });
            }
            address.AddressId = addressId;

            try
            {
                await _IService.DeleteAddress(address);
                var updatedAddress = await _IService.GetAddressByID(addressId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedAddress
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
        /// Delete the address for role customer and owner.
        /// </summary>
        /// <returns>Delete the address with all role.</returns>
        [Authorize(Roles = "owner, customer")]
        [HttpDelete(ApiEndPointConstant.Address.DeleteAddressByToken)]
        public async Task<IActionResult> DeleteAddressByToken([FromBody] DeleteAddressDTO address)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.BAD_REQUEST });
            }
            try
            {
                var allAddresses = await _IService.GetLisAddressByUserID(userId);
                if (allAddresses.Count() == 1)
                {
                    return BadRequest(new { Message = "Không thể xóa địa chỉ. Bạn phải có ít nhất một địa chỉ." });
                }

                await _IService.DeleteAddress(address);
                var updatedAddress = await _IService.GetAddressByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedAddress
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
