using BackendEPPO.Extenstion;
using DTOs.Notification;
using DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Implements;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalletController : ControllerBase
    {
        private readonly IWalletService _service;

        public WalletController(IWalletService IService)
        {
            _service = IService;
        }

        /// <summary>
        /// Get all list wallet 
        /// </summary>
        /// <returns>Get all list wallet </returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Wallet.GetListWallet_Endpoint)]
        public async Task<IActionResult> GetListWallet(int page, int size)
        {
            var _wallet = await _service.GetListWallet(page, size);

            if (_wallet == null || !_wallet.Any())
            {
                return NotFound("No wallet found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _wallet
            });
        }
        /// <summary>
        /// Get wallet by wallet ID for all role 
        /// </summary>
        /// <returns>Get wallet by wallet ID for all role </returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Wallet.GetWalletByID)]
        public async Task<IActionResult> GetWalletByID(int id)
        {
            var _wallet = await _service.GetWalletByID(id);

            if (_wallet == null)
            {
                return NotFound($"Wallet with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _wallet
            });
        }

        /// <summary>
        /// Create wallet for all role 
        /// </summary>
        /// <returns>Create wallet for all role</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Wallet.CreateWallet)]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDTO wallet)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _service.CreateWallet(wallet);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Wallet created successfully",
                Data = wallet
            });
        }

        /// <summary>
        /// Update wallet for all role 
        /// </summary>
        /// <returns>Update wallet for all role</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Wallet.UpdateWalletByID)]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] UpdateWalletDTO wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            wallet.WalletId = id;

            try
            {
                await _service.UpdateWallet(wallet);
                var updatedWallet = await _service.GetWalletByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Wallet updated successfully.",
                    Data = updatedWallet
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Wallet not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
