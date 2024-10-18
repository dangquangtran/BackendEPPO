using BackendEPPO.Extenstion;
using DTOs.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Implements;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PlantController : ControllerBase
    {
        private readonly IPlantService _plantService;

        public PlantController(IPlantService IService)
        {
            _plantService = IService;
        }

        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        //[HttpGet(ApiEndPointConstant.Plants.GetListPlants_Endpoint)]
        //public async Task<IActionResult> GetListPlants(int page, int size)
        //{
        //    var _plant = await _plantService.GetListPlants(page, size);

        //    if (_plant == null || !_plant.Any())
        //    {
        //        return NotFound("No contract found.");
        //    }
        //    return Ok(new
        //    {
        //        StatusCode = 200,
        //        Message = "Request was successful",
        //        Data = _plant
        //    });
        //}

        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        //[HttpGet(ApiEndPointConstant.Plants.GetPlantByID)]
        //public async Task<IActionResult> GetPlantByID(int id)
        //{
        //    var plant = await _plantService.GetPlantByID(id); 

        //    if (plant == null)
        //    {
        //        return NotFound($"Plant with ID {id} not found.");
        //    }
        //    return Ok(new
        //    {
        //        StatusCode = 200,
        //        Message = "Request was successful",
        //        Data = plant
        //    });
        //}

        //[Authorize(Roles = "admin, manager, staff, owner, customer")]
        //[HttpGet(ApiEndPointConstant.Plants.GetPlantByCategory)]
        //public async Task<IActionResult> GetListPlantsByCategory(int Id)
        //{
        //    var _plant = await _plantService.GetListPlantByCategory(Id);

        //    if (_plant == null || !_plant.Any())
        //    {
        //        return NotFound("No contract found.");
        //    }
        //    return Ok(new
        //    {
        //        StatusCode = 200,
        //        Message = "Request was successful",
        //        Data = _plant
        //    });
        //}

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet]
        public IActionResult GetAllPlants(int pageIndex, int pageSize)
        {
            return Ok(_plantService.GetAllPlants(pageIndex, pageSize));
        }

        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetPlantById(int id)
        {
            return Ok(_plantService.GetPlantById(id));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CreatePlant([FromBody] CreatePlantDTO createPlant)
        {
            _plantService.CreatePlant(createPlant);
            return Ok("Đã tạo thành công");
        }

        [Authorize]
        [HttpPut]
        public IActionResult UpdatePlant([FromBody] UpdatePlantDTO updatePlant)
        {
            _plantService.UpdatePlant(updatePlant);
            return Ok("Đã cập nhật thành công");
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet("GetPlantsByCategoryId")]
        public IActionResult GetPlantsByCategoryId(int categoryId, int pageIndex, int pageSize)
        {
            return Ok(_plantService.GetPlantsByCategoryId(categoryId,pageIndex, pageSize));
        }
    }
}
