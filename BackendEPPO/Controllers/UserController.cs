using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService uService)
        {
            _userService = uService;
        }

        [HttpGet(ApiEndPointConstant.User.GetListUsers_Endpoint)]
        public IActionResult GetListUsers()
        {
            return Ok(_userService.GetListUsers());
        }
    }
}
