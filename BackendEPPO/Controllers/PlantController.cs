using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service;
using Service.Implements;
using Service.Interfaces;
using System.Drawing;

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

        [HttpGet]
        public IActionResult GetAllPlants(int pageIndex, int pageSize)
        {
            return Ok(_plantService.GetAllPlants(pageIndex, pageSize));
        }

        [HttpGet("{id}")]
        public IActionResult GetPlantById(int id)
        {
            return Ok(_plantService.GetPlantById(id));
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost]
        public async Task<IActionResult> CreatePlant([FromForm] CreatePlantDTO createPlant)
        {
            await _plantService.CreatePlant(createPlant, createPlant.MainImageFile ,createPlant.ImageFiles);
            return Ok("Đã tạo thành công");
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePlant([FromForm] UpdatePlantDTO updatePlant)
        {
            await _plantService.UpdatePlant(updatePlant, updatePlant.MainImageFile, updatePlant.ImageFiles);
            return Ok("Đã cập nhật thành công");
        }

        /// <summary>
        /// Get list Plant by Category Id with the page and the size.
        /// </summary>
        /// <returns> Get Plant by category id with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetPlantByCategory)]
        public IActionResult GetPlantsByCategoryId( int pageIndex, int pageSize, int categoryId)
        {
            return Ok(_plantService.GetPlantsByCategoryId(pageIndex, pageSize, categoryId));
        }

        /// <summary>
        /// Get list Plant by Type Ecommerce id with the page and the size.
        /// </summary>
        /// <returns>Get Plant by Type Ecommerce id with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceId)]
        public IActionResult GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId)
        {
            return Ok(_plantService.GetListPlantsByTypeEcommerceId(pageIndex, pageSize, typeEcommerceId));
        }

        /// <summary>
        /// Get list Plant by Category Id and Type Ecommerce Id with the page and the size.
        /// </summary>
        /// <returns>Get list Plant by Category Id and Type Ecommerce Id with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceAndCategory)]
        public IActionResult GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId)
        {
            var plant = _plantService.GetListPlantsByTypeEcommerceAndCategory(pageIndex, pageSize, typeEcommerceId, categoryId);

            if (plant == null || !plant.Any())
            {
                return NotFound("No plant found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = plant
            });
        }

        /// <summary>
        /// Search list Plant by key word and Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns>Search list Plant by key word and Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.SearchPlantByKeyWord)]
        public async Task<IActionResult> SearchPlantKeyType(int pageIndex, int pageSize, int typeEcommerceId, string keyword)
        {
            var plants = await _plantService.SearchPlantKeyType(pageIndex, pageSize, typeEcommerceId, keyword);

            if (plants == null || !plants.Any())
            {
                return NotFound("No plant found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = plants
            });
        
        }

        /// <summary>
        ///  Check Plant in to the cart is new value.
        /// </summary>
        /// <returns>Check Plant in to the cart is new value.</returns>
        [HttpGet(ApiEndPointConstant.Plants.CheckPlantInCart)]
        public async Task<IActionResult> CheckPlantInCart(List<int> plantId)
        {
            var plants = await _plantService.CheckPlantInCart(plantId);

            if (plants == null || !plants.Any())
            {
                return NotFound("No plant found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = plants
            });

        }

        [HttpGet("search")]
        public IActionResult SearchPlants(string keyword, int pageIndex, int pageSize)
        {
            var plants = _plantService.SearchPlants(keyword, pageIndex, pageSize);
            
            return Ok(plants);
        }
    }
}
