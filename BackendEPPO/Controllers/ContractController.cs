using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharp;
using Service.Interfaces;
using System.Reflection.Metadata;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
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

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Contract.CreateContract)]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDTO contract)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contractService.CreateContract(contract);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Contract created successfully",
                Data = contract
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.Contract.UpdateContractID)]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractDTO contract)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            contract.ContractId = id;

            try
            {
                await _contractService.UpdateContract(contract);
                var updatedContract = await _contractService.GetContractByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Contract updated successfully.",
                    Data = updatedContract
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contract not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }


       
    }
}
