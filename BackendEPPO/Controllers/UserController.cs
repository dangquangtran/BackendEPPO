using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Mvc;
using Service;
using System.Threading.Tasks;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet(ApiEndPointConstant.User.GetListUsers_Endpoint)]
        public async Task<IActionResult> GetListUsers()
        {
            var users = await _userService.GetListUsers();

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(new
            {
                StatusCode = 200,  
                Message = "Request was successful",
                Data = users
            });
        }
    }
}
