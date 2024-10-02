using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost(ApiEndPointConstant.User.Login_Endpoint)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IActionResult response = Unauthorized();
            var user = _userService.GetAllUsers().Where(x => x.UserName == request.UserName).FirstOrDefault();

            if (user != null && user.Password == request.Password)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { token = tokenString, role = user.Role });
            }
            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: new[]
                {
                    new Claim(ClaimTypes.Name, userInfo.FullName),
                    new Claim(ClaimTypes.Email, userInfo.Email),
                    new Claim(ClaimTypes.Role, userInfo.Role.NameRole.ToString()),
                    new Claim("userId", userInfo.UserId.ToString())
                },
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
