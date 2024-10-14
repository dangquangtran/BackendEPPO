using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {

        private readonly IBlogService _service;

        public BlogController(IBlogService IService)
        {
            _service = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Blog.GetListBlog_Endpoint)]
        public async Task<IActionResult> GetListBlog(int page, int size)
        {
            var _blog = await _service.GetListBlog(page, size);

            if (_blog == null || !_blog.Any())
            {
                return NotFound("No Blog found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _blog
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Blog.GetBlogByID)]
        public async Task<IActionResult> GetBlogByID(int id)
        {
            var _blog = await _service.GetBlogByID(id);

            if (_blog == null)
            {
                return NotFound($"Blog with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _blog
            });
        }
    }
}
