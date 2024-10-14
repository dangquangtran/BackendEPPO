using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserVouchersController : ControllerBase
    {
        private readonly IUserVoucherService _service;

        public UserVouchersController(IUserVoucherService IService)
        {
            _service = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserVoucher.GetListUserVoucher_Endpoint)]
        public async Task<IActionResult> GetListUserVoucher(int page, int size)
        {
            var _userVoucher = await _service.GetListUserVoucher(page, size);

            if (_userVoucher == null || !_userVoucher.Any())
            {
                return NotFound("No user voucher found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _userVoucher
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.UserVoucher.GetImageFeedbackByID)]
        public async Task<IActionResult> GetUserVoucherByID(int id)
        {
            var _userVoucher = await _service.GetUserVoucherByID(id);

            if (_userVoucher == null)
            {
                return NotFound($"User Voucher with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = _userVoucher
            });
        }
    }
}
