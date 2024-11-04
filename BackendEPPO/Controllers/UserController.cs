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

        [Authorize(Roles = "admin, manager, staff")]
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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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
                Message = "Customer created successfully",
                Data = customer
            });
        }

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



    }
}
