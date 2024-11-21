using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Error;
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
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = users
            });
        }

        /// <summary>
        /// Filter list account by role in database with the page and the size.
        /// </summary>
        /// <returns>Get list the staff in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager")]
        [HttpGet(ApiEndPointConstant.User.GetListFilterByRole_Endpoint)]
        public async Task<IActionResult> FilterAccountByRoleID(int page, int size ,int roleId)
        {
            var users = await _userService.FilterAccountByRoleID(page, size, roleId);

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = users
            });
        }

        /// <summary>
        /// Function of manager: Search account by key work with the page and the size.
        /// </summary>
        /// <returns>Function of manager: Search account by key work with the page and the size.</returns>
        [Authorize(Roles = "admin, manager")]
        [HttpGet(ApiEndPointConstant.User.SearchAccountByKey_Endpoint)]
        public async Task<IActionResult> SearchAccountByKey(int page, int size, string keyWord)
        {
            var users = await _userService.SearchAccountByKey(page, size, keyWord);

            if (users == null || !users.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = users
            });
        }

        /// <summary>
        /// Get information User by userID for role admin, manager,staff.
        /// </summary>
        /// <returns>Get information User by userID for role admin, manager,staff.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.User.GetUserByID)]
        public async Task<IActionResult> GetUsersByID(int userId)
        {
            var users = await _userService.GetUsersByID(userId);

            if (users == null)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
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
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
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
                    Message = Error.CREATE_ACCOUNT_FAIL,
                });
            }
            await _userService.CreateAccountByCustomer(customer);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.CREATE_ACCOUNT_SUCCESSFUL,
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
            bool isExistingUser = await _userService.CheckAccountExists(owner.Email, owner.UserName);

            if (isExistingUser)
            {
                return Conflict(new
                {
                    StatusCode = 409,
                    Message = Error.CREATE_ACCOUNT_FAIL,
                });
            }
            await _userService.CreateAccountByOwner(owner);

            return Ok(new
            {
                StatusCode = 201,
                Message = Error.REQUESR_SUCCESFULL,
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
                Message = Error.REQUESR_SUCCESFULL,
                Data = admin
            });
        }

        /// <summary>
        /// Update information account by user Id .
        /// </summary>
        /// <returns>Update information account by all role manager.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.User.UpdateAccount)]
        public async Task<IActionResult> UpdateUserAccount(int userId, [FromForm] UpdateAccount accountDTO)
        {
            if (!ModelState.IsValid)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            accountDTO.UserId = userId;

            try
            {
                await _userService.UpdateUserAccount(accountDTO, accountDTO.ImageFile);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedUser
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
        /// Update information account by token for mobile role customer and owner.
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
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
         
            try
            {
                await _userService.UpdateInformationAccount(accountDTO, accountDTO.ImageFile , userId);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.UPDATE_ACCOUNT_SUCCESSFUL,
                    Data = updatedUser
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
        /// Change password of account by user Id.
        /// </summary>
        /// <returns>Change password of account by all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.User.ChangePassword)]
        public async Task<IActionResult> ChangePasswordAccount(int userId, [FromBody] ChangePassword accountDTO)
        {
            if (!ModelState.IsValid)
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            accountDTO.UserId = userId;

            try
            {
                await _userService.ChangePasswordAccount(accountDTO);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.UPDATE_ACCOUNT_SUCCESSFUL,
                    Data = updatedUser
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
        /// Change password of account by token.
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
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            accountDTO.UserId = userId;
            try
            {
                await _userService.ChangePasswordAccount(accountDTO);
                var updatedUser = await _userService.GetUsersByID(userId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.UPDATE_ACCOUNT_SUCCESSFUL,
                    Data = updatedUser
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
