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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Wallet.GetWalletByID)]
        public async Task<IActionResult> GetWalletByID(int id)
        {
            var _wallet = await _service.GetWalletByID(id);

            if (_wallet == null)
            {
                return NotFound(new
                {
                    StatusCode = 404,
                    Message = $"Không tìm thấy ví có ID {id}.",
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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Wallet.UpdateWalletByID)]
        public async Task<IActionResult> UpdateWallet(int id, [FromBody] UpdateWalletDTO wallet)
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
            wallet.WalletId = id;

            try
            {
                await _service.UpdateWallet(wallet);
                var updatedWallet = await _service.GetWalletByID(id);

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
