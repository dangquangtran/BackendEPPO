using BackendEPPO.Extenstion;
using DTOs.ContractDetails;
using DTOs.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
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
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.ContractDetails.CreateContractDetails)]
        public async Task<IActionResult> CreateContractDetail([FromBody] CreateContractDetailDTO contractDetail)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contractDetailsService.CreateContractDetail(contractDetail);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Contract Detail created successfully",
                Data = contractDetail
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.ContractDetails.UpdateContractDetailsID)]
        public async Task<IActionResult> UpdateContractDetail(int id, [FromBody] UpdateContractDetailDTO contractDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            contractDetail.ContractDetailId = id;

            try
            {
                await _contractDetailsService.UpdateContractDetail(contractDetail);
                var updatedContractDetail = await _contractDetailsService.GetContractDetailByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Contract Detail updated successfully.",
                    Data = updatedContractDetail
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Contract Detail not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
    }
}
