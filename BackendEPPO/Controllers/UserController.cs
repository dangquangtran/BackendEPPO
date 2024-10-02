using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.User;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;
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
        public async Task<IActionResult> GetListUsers(int page, int size)
        {
            var users = await _userService.GetListUsers(page, size);

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

        [HttpGet(ApiEndPointConstant.User.GetUserByID)]
        public async Task<IActionResult> GetUsersByID(int id)
        {
            var users = await _userService.GetUsersByID(id);

            if (users == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = users
            });
        }

        [HttpPost(ApiEndPointConstant.User.CreateUserAccount)]
        public async Task<IActionResult> CreateUser([FromBody] ResponseUserDTO user)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateUserAccount(user);

            return Ok(new
            {
                StatusCode = 201,
                Message = "User created successfully",
                Data = user
            });
        }


    }
}
