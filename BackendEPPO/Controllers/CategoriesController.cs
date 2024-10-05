using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Categories.GetListCategory_Endpoint)]
        public async Task<IActionResult> GetListCategory(int page, int size)
        {
            var _cate = await _IService.GetListCategory(page, size);

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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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
