using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
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

    }
}
