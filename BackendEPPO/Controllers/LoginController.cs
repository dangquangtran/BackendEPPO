using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Login;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Service.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DTOs.User;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //1-admin
    //2-manage
    //3-staff
    //4-customer
    //5-owner
    public class LoginController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        public LoginController(IUserService userService, IConfiguration configuration, ITokenService tokenService)
        {
            _userService = userService;
            _configuration = configuration;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Login with user name password or login with account email  
        /// </summary>
        /// <returns>Login with user name password or login with account email. </returns>
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

            //var user = _userService.GetAllUsers().Where(x => x.Email == request.UsernameOrEmail).FirstOrDefault();


            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
          

            if (user != null && user.Password == request.Password)
            {
                var tokenString = GenerateJSONWebToken(user);

                response = Ok(new
                {
                    StatusCode = 201,
                    Message = "Đăng nhập thành công",
                    token = tokenString,
                    //userID = user.UserId,
                    roleName = user.Role.NameRole,
                    level = user.RankLevel,
                    isSigned = user.IsSigned,
                    
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
                    new Claim("userId", userInfo.UserId.ToString()),
                    new Claim("roleId", userInfo.RoleId.ToString()),
                    new Claim("roleName", userInfo.Role.NameRole),
                    new Claim("fullName", userInfo.FullName.ToString()),
                    new Claim("email", userInfo.Email.ToString()),
                    new Claim("phoneNumber", userInfo.PhoneNumber.ToString()),
                    new Claim("walletId", userInfo.WalletId.ToString()),
                   
                    new Claim(ClaimTypes.Role, userInfo.Role.NameRole),

                }, 
                expires: DateTime.UtcNow.AddHours(7).AddMinutes(120),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [AllowAnonymous]
        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            // Redirects to Google for login
            var redirectUrl = Url.Action("GoogleResponse", "Login");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("GoogleResponse")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal == null)
                return BadRequest("Error signing in with Google");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            var user = _userService.GetAllUsers().FirstOrDefault(u => u.Email == email);
            if (user == null)
            {
                // Optionally, create a new user if one does not already exist
                var newUser = new ResponseUserDTO
                {
                    Email = email,
                    FullName = name,
                    UserName = email, // or any other unique identifier
                    CreationDate = DateTime.UtcNow.AddHours(7),
                    IsActive = true,
                    Status = 1,
                    RoleId = 4
                };
                _userService.CreateUserAccount(newUser);
            }

            var tokenString = GenerateJSONWebToken(user);
            return Ok(new { token = tokenString });
        }

        /// <summary>
        /// Login with Face Id   
        /// </summary>
        /// <returns>Login with user name password or login with account email. </returns>
        [AllowAnonymous]
        [HttpPost(ApiEndPointConstant.User.Login_FaceID_Endpoint)]
        public IActionResult LoginWithFaceId([FromForm] FaceIdLoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest(new { message = "Missing Face ID token" });
            }

            // Validate token và lấy userId từ token
            var userId = _tokenService.ValidateToken(request.Token);
            if (!userId.HasValue)
            {
                return Unauthorized(new { message = "Invalid Face ID token" });
            }

            // Lấy thông tin người dùng từ DB
            var user = _userService.GetUserByID(userId.Value); // Đảm bảo `GetUserByID` chấp nhận `int`
            if (user == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            // Tạo JWT mới
            var tokenString = GenerateJSONWebToken(user);

            // Trả về thông tin đăng nhập thành công
            return Ok(new
            {
                StatusCode = 200,
                Message = "Đăng nhập bằng Face ID thành công",
                Token = tokenString,
                RoleName = user.Role.NameRole,
                FullName = user.FullName,
                IsSigned = user.IsSigned
            });
        }
        public class FaceIdLoginRequest
        {
            public string Token { get; set; }
        }

    }
}
