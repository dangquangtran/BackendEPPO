using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                return NotFound("No wallet found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
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
                return NotFound($"Wallet with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _wallet
            });
        }
    }
}
