using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.User;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Get list all Users in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Users in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.User.GetListUsers_Endpoint)]
        public async Task<IActionResult> GetListUsers(int page, int size)
        {
            var users = await _userService.GetListUsers(page, size);

            if (users == null || !users.Any())
            {
                return NotFound("Không tìm thấy dữ liệu.");
            }
            return Ok(new
            {
                StatusCode = 200,  
                Message = "Tra cứu dữ liệu thành công.",
                Data = users
            });
        }

        /// <summary>
        /// Get information User by userID for role admin, manager,staff.
        /// </summary>
        /// <returns>Get information User by userID for role admin, manager,staff.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.User.GetUserByID)]
        public async Task<IActionResult> GetUsersByID(int id)
        {
            var users = await _userService.GetUsersByID(id);

            if (users == null)
            {
                return NotFound($"Người dùng có Id = {id} không tồn tại.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Tra cứu dữ liệu thành công.",
                Data = users
            });
        }

        /// <summary>
        /// Get information by token for role customer and owner.
        /// </summary>
        /// <returns>Get information by token for role customer and owner.</returns>
        [Authorize(Roles = "customer, owner")]
        [HttpGet(ApiEndPointConstant.User.GetInformationByID)]
        public async Task<IActionResult> GetInformationByID()
        {

            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            var users = await _userService.GetUsersByID(userId);

            if (users == null)
            {
                return NotFound($"Người dùng có Id = {userId} không tồn tại.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = users
            });
        }

        /// <summary>
        /// Create account user for customer and guest by role customer.
        /// </summary>
        /// <returns> Create account user for customer and guest by role customer.</returns>
        [HttpPost(ApiEndPointConstant.User.CreateAccountByCustomer)]
        public async Task<IActionResult> CreateAccountByCustomers([FromBody] CreateAccountByCustomerDTO customer)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            bool isExistingUser = await _userService.CheckAccountExists(customer.Email, customer.UserName);

            if (isExistingUser)
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Message = "Account with the same email or username already exists."
                });
            }
            await _userService.CreateAccountByCustomer(customer);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Tạo tài khoản thành công.",
                Data = customer
            });
        }

        /// <summary>
        /// Create account user for owner by role owner.
        /// </summary>
        /// <returns>Create account user for owner by role owner.</returns>
        [HttpPost(ApiEndPointConstant.User.CreateAccountByOwner)]
        public async Task<IActionResult> CreateAccountByOwner([FromBody] CreateAccountByOwnerDTO owner)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateAccountByOwner(owner);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Owner created successfully",
                Data = owner
            });
        }


        /// <summary>
        /// Create account user for staff by role admin.
        /// </summary>
        /// <returns>Create account user for staff by role admin.</returns>
        [HttpPost(ApiEndPointConstant.User.CreateAccountByAdmin)]
        public async Task<IActionResult> CreateAccountByAdmin([FromBody] CreateAccountByAdminDTO admin)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _userService.CreateAccountByAdmin(admin);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Admin created successfully",
                Data = admin
            });
        }

        /// <summary>
        /// Update information account by all role manager.
        /// </summary>
        /// <returns>Update information account by all role manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.User.UpdateAccount)]
        public async Task<IActionResult> UpdateUserAccount(int id, [FromForm] UpdateAccount accountDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            accountDTO.UserId = id;

            try
            {
                await _userService.UpdateUserAccount(accountDTO, accountDTO.ImageFile);
                var updatedUser = await _userService.GetUsersByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "User account updated successfully.",
                    Data = updatedUser
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Update information account by for mobile role customer and owner.
        /// </summary>
        /// <returns>Update information account by for mobile.</returns>
        [Authorize(Roles = "owner, customer")]
        [HttpPut(ApiEndPointConstant.User.UpdateInformationAccount)]
        public async Task<IActionResult> UpdateInformationAccount([FromForm] UpdateInformation accountDTO)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
         
            try
            {
                await _userService.UpdateInformationAccount(accountDTO, accountDTO.ImageFile , userId);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "User account updated successfully.",
                    Data = updatedUser
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Change password of account by all role.
        /// </summary>
        /// <returns>Change password of account by all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.User.ChangePassword)]
        public async Task<IActionResult> ChangePasswordAccount(int id, [FromBody] ChangePassword accountDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            accountDTO.UserId = id;

            try
            {
                await _userService.ChangePasswordAccount(accountDTO);
                var updatedUser = await _userService.GetUsersByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "User account updated successfully.",
                    Data = updatedUser
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

        /// <summary>
        /// Change password of account by all role.
        /// </summary>
        /// <returns>Change password of account by all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.User.ChangePasswordByToken)]
        public async Task<IActionResult> ChangePasswordByToken(ChangePassword accountDTO)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            accountDTO.UserId = userId;

            try
            {
                await _userService.ChangePasswordAccount(accountDTO);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "User account updated successfully.",
                    Data = updatedUser
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "User not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }

    }
}
