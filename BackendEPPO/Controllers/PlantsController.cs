using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantsController : ControllerBase
    {
        private readonly IPlantService _plantsService;

        public PlantsController(IPlantService IService)
        {
            _plantsService = IService;
        }

        [HttpGet(ApiEndPointConstant.Plants.GetListPlants_Endpoint)]
        public async Task<IActionResult> GetListPlants()
        {
            var _plant = await _plantsService.GetListPlants();

            if (_plant == null || !_plant.Any())
            {
                return NotFound("No contract found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _plant
            });
        }
    }
}
