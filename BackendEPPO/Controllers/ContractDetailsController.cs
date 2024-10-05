using BackendEPPO.Extenstion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractDetailsController : ControllerBase
    {
        private readonly IContractDetailServices _contractDetailsService;

        public ContractDetailsController(IContractDetailServices IService)
        {
            _contractDetailsService = IService;
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ContractDetails.GetListContractDetails_Endpoint)]
        public async Task<IActionResult> GetListContractDetail(int page, int size)
        {
            var contractDetail = await _contractDetailsService.GetListContractDetail(page, size);

            if (contractDetail == null || !contractDetail.Any())
            {
                return NotFound("No contract detail found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = contractDetail
            });
        }
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ContractDetails.GetContractDetailsByID)]
        public async Task<IActionResult> GetContractDetailByID(int id)
        {
            var contractDetail = await _contractDetailsService.GetContractDetailByID(id);

            if (contractDetail == null)
            {
                return NotFound($"Contract Detail with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = contractDetail
            });
        }
    }
}
