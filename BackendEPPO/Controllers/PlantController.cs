using BackendEPPO.Extenstion;
using DTOs.Plant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [HttpGet]
        public IActionResult GetAllPlants(int pageIndex, int pageSize)
        {
            var plants = _plantService.GetAllPlants(pageIndex, pageSize);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetPlantById(int id)
        {
            var plant = _plantService.GetPlantById(id);
            if (plant == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Không tìm thấy cây với ID {id}.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plant
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost]
        public async Task<IActionResult> CreatePlant([FromForm] CreatePlantDTO createPlant)
        {
            await _plantService.CreatePlant(createPlant, createPlant.MainImageFile, createPlant.ImageFiles);
            return Ok(new
            {
                StatusCode = 201,
                Message = "Đã tạo cây thành công.",
                Data = createPlant
            });
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdatePlant([FromForm] UpdatePlantDTO updatePlant)
        {
            await _plantService.UpdatePlant(updatePlant, updatePlant.MainImageFile, updatePlant.ImageFiles);
            return Ok(new
            {
                StatusCode = 200,
                Message = "Đã cập nhật cây thành công.",
                Data = updatePlant
            });
        }

        /// <summary>
        /// Get list Plant by Category with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Category with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetPlantByCategory)]
        public IActionResult GetPlantsByCategoryId(int pageIndex, int pageSize, int categoryId)
        {
            var plants = _plantService.GetPlantsByCategoryId(pageIndex, pageSize, categoryId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào trong danh mục.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }


        /// <summary>
        /// Get list Plant by Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceId)]
        public IActionResult GetListPlantsByTypeEcommerceId(int pageIndex, int pageSize, int typeEcommerceId)
        {
            var plants = _plantService.GetListPlantsByTypeEcommerceId(pageIndex, pageSize, typeEcommerceId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Get list Plant by Type Ecommerce And Category with the page and the size.
        /// </summary>
        /// <returns> Get list Plant by Type Ecommerce And Category with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.GetListPlantsByTypeEcommerceAndCategory)]
        public IActionResult GetListPlantsByTypeEcommerceAndCategory(int pageIndex, int pageSize, int typeEcommerceId, int categoryId)
        {
            var plants = _plantService.GetListPlantsByTypeEcommerceAndCategory(pageIndex, pageSize, typeEcommerceId, categoryId);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo loại thương mại điện tử và danh mục.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        /// <summary>
        /// Search list Plant by key word and Type Ecommerce with the page and the size.
        /// </summary>
        /// <returns>Search list Plant by key word and Type Ecommerce with the page and the size.</returns>
        [HttpGet(ApiEndPointConstant.Plants.SearchPlantByKeyWord)]
        public async Task<IActionResult> SearchPlantKeyType(int pageIndex, int pageSize, int typeEcommerceId, string keyWord)
        {
            var plants = await _plantService.SearchPlantKeyType(pageIndex, pageSize, typeEcommerceId, keyWord);

            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa."
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
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
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa."
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }

        [HttpGet("search")]
        public IActionResult SearchPlants(string keyword, int typeEcommerceId, int pageIndex, int pageSize)
        {
            var plants = _plantService.SearchPlants(keyword, typeEcommerceId, pageIndex, pageSize);
            if (plants == null || !plants.Any())
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy cây nào theo từ khóa.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = plants
            });
        }
    }
}
