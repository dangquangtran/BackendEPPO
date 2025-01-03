﻿using BackendEPPO.Extenstion;
using DTOs.ContractDetails;
using DTOs.Contracts;
using DTOs.Error;
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

        /// <summary>
        /// Get list all Contract details in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Contract details in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ContractDetails.GetListContractDetails_Endpoint)]
        public async Task<IActionResult> GetListContractDetail(int page, int size)
        {
            var contractDetail = await _contractDetailsService.GetListContractDetail(page, size);

            if (contractDetail == null || !contractDetail.Any())
            {
                return NotFound(new { Message = Error.NO_DATA_FOUND });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = contractDetail
            });
        }

        /// <summary>
        /// Get Contract details by contract detail id.
        /// </summary>
        /// <returns>Get Contract details by contract detail id.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.ContractDetails.GetContractDetailsByID)]
        public async Task<IActionResult> GetContractDetailByID(int contractDetailByID)
        {
            var contractDetail = await _contractDetailsService.GetContractDetailByID(contractDetailByID);

            if (contractDetail == null)
            {
                return BadRequest(new { Message = Error.REQUESR_SUCCESFULL });
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = Error.REQUESR_SUCCESFULL,
                Data = contractDetail
            });
        }

        /// <summary>
        /// Create Contract details with all role.
        /// </summary>
        /// <returns>Create Contract details with all role.</returns>
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
                Message = Error.REQUESR_SUCCESFULL,
                Data = contractDetail
            });
        }

        /// <summary>
        /// Update Contract details with all role.
        /// </summary>
        /// <returns>Update Contract details with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPut(ApiEndPointConstant.ContractDetails.UpdateContractDetailsID)]
        public async Task<IActionResult> UpdateContractDetail(int id, [FromBody] UpdateContractDetailDTO contractDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Error.REQUESR_SUCCESFULL });
            }
            contractDetail.ContractDetailId = id;

            try
            {
                await _contractDetailsService.UpdateContractDetail(contractDetail);
                var updatedContractDetail = await _contractDetailsService.GetContractDetailByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = Error.REQUESR_SUCCESFULL,
                    Data = updatedContractDetail
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
