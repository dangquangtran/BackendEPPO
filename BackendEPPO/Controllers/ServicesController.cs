using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceService _servicesService;

        public ServicesController(IServiceService IService)
        {
            _servicesService = IService;
        }

        [HttpGet(ApiEndPointConstant.Services.GetListServices_Endpoint)]
        public async Task<IActionResult> GetListServices()
        {
            var services = await _servicesService.GetListServices();

            if (services == null || !services.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = services
            });
        }
    }
}
