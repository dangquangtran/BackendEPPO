using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessoryController : ControllerBase
    {
        private readonly IAccessoryService _accessoryService;

        public AccessoryController(IAccessoryService IService)
        {
            _accessoryService = IService;
        }

        [HttpGet(ApiEndPointConstant.Accessory.GetListAccessory_Endpoint)]
        public async Task<IActionResult> GetListAccessory()
        {
            var _accessory = await _accessoryService.GetListAccessory();

            if (_accessory == null || !_accessory.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _accessory,
            });
        }
    }
}
