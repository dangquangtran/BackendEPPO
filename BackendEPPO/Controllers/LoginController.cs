using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Login;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //1-admin
    //2-manage
    //3-staff
    //4-customer
    //5-owwner
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public LoginController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost(ApiEndPointConstant.User.Login_Endpoint)]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            IActionResult response = Unauthorized();

            var user = _userService.GetAllUsers()
                .FirstOrDefault(x =>
                    x.Email.Equals(request.UsernameOrEmail, StringComparison.OrdinalIgnoreCase) ||
                    x.UserName.Equals(request.UsernameOrEmail, StringComparison.OrdinalIgnoreCase));

            
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid username/email or password" });
            }


            //var user = _userService.GetAllUsers().Where(x => x.Email == request.Email).FirstOrDefault();


            if (user != null && user.Password == request.Password)
            {
                var tokenString = GenerateJSONWebToken(user);

                response = Ok(new
                {
                    token = tokenString,
                });
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
                    //new Claim("userId", userInfo.UserId.ToString()),
                    //new Claim("roleId", userInfo.RoleId.ToString()),
                    //new Claim("roleName", userInfo.Role.NameRole),
                    //new Claim("fullName", userInfo.FullName.ToString()),
                    //new Claim("email", userInfo.Email.ToString()),
                    //new Claim("phoneNumber", userInfo.PhoneNumber.ToString()),
                    //new Claim("gender", userInfo.Gender.ToString()),
                    //new Claim("rankId", userInfo.RankId.ToString()),
                    //new Claim("walletId", userInfo.WalletId.ToString()),
                    //new Claim("identificationCard", userInfo.IdentificationCard.ToString()),
                    //new Claim("dateOfBirth", userInfo.DateOfBirth.ToString()),
                    //new Claim(ClaimTypes.Role, userInfo.Role.NameRole),

                    new Claim("userId", userInfo.UserId.ToString()),
                    new Claim("roleId", userInfo.RoleId.ToString()),
                    new Claim("roleName", userInfo.Role.NameRole ?? "Unknown"),
                    new Claim("fullName", userInfo.FullName ?? string.Empty),
                    new Claim("email", userInfo.Email ?? string.Empty),
                    new Claim("phoneNumber", userInfo.PhoneNumber ?? string.Empty),
                    new Claim("gender", userInfo.Gender?.ToString() ?? string.Empty),
                    new Claim("rankId", userInfo.RankId?.ToString() ?? "0"),
                    new Claim("walletId", userInfo.WalletId?.ToString() ?? "0"),
                    new Claim("identificationCard", userInfo.IdentificationCard.ToString() ?? "0"),
                    new Claim("dateOfBirth", userInfo.DateOfBirth?.ToString("yyyy-MM-dd") ?? string.Empty),
                    new Claim(ClaimTypes.Role, userInfo.Role.NameRole ?? "Unknown")

                }, 
                expires: DateTime.Now.AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
