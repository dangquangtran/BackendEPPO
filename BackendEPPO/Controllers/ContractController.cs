using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractController(IContractService IService)
        {
            _contractService = IService;
        }

        [HttpGet(ApiEndPointConstant.Contract.GetListContract_Endpoint)]
        public async Task<IActionResult> GetListContracts(int page, int size)
        {
            var contract = await _contractService.GetListContract(page, size);

            if (contract == null || !contract.Any())
            {
                return NotFound("No contract found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = contract
            });
        }
        [HttpGet(ApiEndPointConstant.Contract.GetContractByID)]
        public async Task<IActionResult> GetUsersByID(int id)
        {
            var contract = await _contractService.GetContractByID(id);

            if (contract == null)
            {
                return NotFound($"Contract with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = contract
            });
        }
    }
}
