using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _IService;

        public CategoriesController(ICategoryService IService)
        {
            _IService = IService;
        }

        [HttpGet(ApiEndPointConstant.Categories.GetListCategory_Endpoint)]
        public async Task<IActionResult> GetListCategory()
        {
            var _cate = await _IService.GetListCategory();

            if (_cate == null || !_cate.Any())
            {
                return NotFound("No contract found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _cate
            });
        }

        [HttpGet(ApiEndPointConstant.Categories.GetCategoriesByID)]
        public async Task<IActionResult> GetCategoriesByID(int id)
        {
            var _cate = await _IService.GetCategoryByID(id);

            if (_cate == null)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _cate
            });
        }
    }
}
