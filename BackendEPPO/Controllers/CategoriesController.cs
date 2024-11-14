using BackendEPPO.Extenstion;
using DTOs.Category;
using DTOs.Error;
using DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Service.Interfaces;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

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

        /// <summary>
        /// Get list all Category in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Category in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Categories.GetListCategory_Endpoint)]
        public async Task<IActionResult> GetListCategory(int page, int size)
        {
            var _cate = await _IService.GetListCategory(page, size);

            if (_cate == null || !_cate.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = _cate
            });
        }

        /// <summary>
        /// Get Category by categoryID with all role.
        /// </summary>
        /// <returns>Get Category by categoryID with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Categories.GetCategoriesByID)]
        public async Task<IActionResult> GetCategoriesByID(int id)
        {
            var _cate = await _IService.GetCategoryByID(id);

            if (_cate == null)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = _cate
            });
        }

        /// <summary>
        /// Create Category with role manager and staff.
        /// </summary>
        /// <returns>Create Category with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpPost(ApiEndPointConstant.Categories.CreateCategories)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO category)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _IService.CreateCategory(category);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
                Data = category
            });
        }


        /// <summary>
        /// Update Category with role manager and staff.
        /// </summary>
        /// <returns>Update Category with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Categories.UpdateCategoriesID)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.REQUESR_SUCCESFULL });
            }
            category.CategoryId = id;

            try
            {
                await _IService.UpdateCategory(category);
                var updatedcategory = await _IService.GetCategoryByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedcategory
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
        /// Delete Category with role manager and staff.
        /// </summary>
        /// <returns>Delete Category with role manager and staff.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpDelete(ApiEndPointConstant.Categories.DeleteCategoriesID)]
        public async Task<IActionResult> DeleteCategory(int id, [FromBody] DeleteCategoryDTO category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.REQUESR_SUCCESFULL });
            }
            category.CategoryId = id;

            try
            {
                await _IService.DeleteCategory(category);
                var updatedcategory = await _IService.GetCategoryByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedcategory
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
