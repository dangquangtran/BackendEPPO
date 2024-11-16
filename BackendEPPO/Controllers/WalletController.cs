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
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy ví nào.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = _wallet
            });
        }
        /// <summary>
        /// Get wallet by wallet ID for all role 
        /// </summary>
        /// <returns>Get wallet by wallet ID for all role </returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Wallet.GetWalletByID)]
        public async Task<IActionResult> GetWalletByID(int walletID)
        {
            var _wallet = await _service.GetWalletByID(walletID);

            if (_wallet == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Không tìm thấy ví có ID {walletID}.",
                    Data = (object)null
                });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Yêu cầu thành công.",
                Data = _wallet
            });
        }
        /// <summary>
        /// Get the the transaction  with all role.
        /// </summary>
        /// <returns>Get the the transaction  with all role.</returns>
      //  [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Wallet.GetListTransaction_Endpoint)]
        public async Task<IActionResult> GetListTransactionsByWallet()
        {

            var walletIdClaim = User.FindFirst("walletId")?.Value;
            int walletId = int.Parse(walletIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không tồn tại." });
            }
            try
            {
                var transaction = await _service.GetListTransactionsByWallet(walletId);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Truy van",
                    Data = transaction
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Không tìm thấy địa chỉ" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi 500:", error = ex.Message });
            }

        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Wallet.CreateWallet)]
        public async Task<IActionResult> CreateWallet([FromBody] CreateWalletDTO wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Data = ModelState
                });
            }

            await _service.CreateWallet(wallet);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Tạo ví mới thành công.",
                Data = wallet
            });
        }

        /// <summary>
        /// Update wallet for all role 
        /// </summary>
        /// <returns>Update wallet for all role</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Wallet.UpdateWalletByID)]
        public async Task<IActionResult> UpdateWallet(int walletId, [FromBody] UpdateWalletDTO wallet)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Dữ liệu đầu vào không hợp lệ.",
                    Data = ModelState
                });
            }
            wallet.WalletId = walletId;

            try
            {
                await _service.UpdateWallet(wallet);
                var updatedWallet = await _service.GetWalletByID(walletId);

                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Cập nhật ví thành công.",
                    Data = updatedWallet
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy ví.",
                    Data = (object)null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    StatusCode = 500,
                    Message = "Đã xảy ra lỗi.",
                    Error = ex.Message
                });
            }
        }
    }
}
