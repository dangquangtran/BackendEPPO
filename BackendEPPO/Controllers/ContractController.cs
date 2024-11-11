using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.Wallet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PdfSharp;
using Service.Implements;
using Service.Interfaces;
using System.Collections.Generic;
using System.Reflection.Metadata;
using static BackendEPPO.Extenstion.ApiEndPointConstant;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly FirebaseStorageService _firebaseStorageService;

        public ContractController(IContractService IService, FirebaseStorageService firebaseStorageService)
        {
            _contractService = IService;
            _firebaseStorageService = firebaseStorageService;
        }

        /// <summary>
        /// Get list all Contracts of UserId in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Contracts in database with the page and the size.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Contract.GetContractOfUser_Endpoint)]
        public async Task<IActionResult> GetContractOfUser(int userID)
        {
            var users = await _contractService.GetContractOfUser(userID);

            if (users == null || !users.Any())
            {
                return NotFound("No users found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = users
            });
        }

        /// <summary>
        /// Get list all Contracts in database with the page and the size.
        /// </summary>
        /// <returns>Get list all Contracts in database with the page and the size.</returns>
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

        /// <summary>
        /// Get Contracts by ContractID with all role.
        /// </summary>
        /// <returns>Get Contracts with ContractID with all role.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Contract.GetContractByID)]
        public async Task<IActionResult> GetContractByID(int id)
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

        /// <summary>
        /// Create Contracts with all role for Renting.
        /// </summary>
        /// <returns>Create Contracts with all role for Renting.</returns>
        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpPost(ApiEndPointConstant.Contract.CreateContract)]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractDTO contract)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contractService.CreateContract(contract, userId);
            string contractPdfUrl = await _contractService.GenerateContractPdfAsync(contract);
  

            return Ok(new
            {
                StatusCode = 201,
                Message = "Contract created successfully",
                PdfUrl = contractPdfUrl,
                Data = contract,

            });
        }

        /// <summary>
        /// Create Contracts with role register owner.
        /// </summary>
        /// <returns>Create Contracts with role register owner.</returns>
        [Authorize(Roles = "admin, manager, staff, owner")]
        [HttpPost(ApiEndPointConstant.Contract.CreatePartnershipContract)]
        public async Task<IActionResult> CreatePartnershipContract([FromBody] CreateContractPartnershipDTO contracts)
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _contractService.CreatePartnershipContract(contracts, userId);

            string contractPdfUrl = await _contractService.GenerateBusinessPartnershipContractPdfAsync(contracts, userId);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Contract created successfully",
                PdfUrl = contractPdfUrl,
                Data = contracts,

            });
        }


        /// <summary>
        /// Download contract details with all role.
        /// </summary>
        /// <returns>Download contract with all role.</returns>
        [HttpGet("contracts/{fileName}")]
        //[HttpGet(ApiEndPointConstant.Contract.DownLoadContract)]
        public async Task<IActionResult> DownloadContractPdf(string fileName)
        {
            // Chỉ định tên thư mục trên Firebase Storage
            string folderName = "contracts";

            try
            {
                // Tải file từ Firebase Storage
                var fileStream = await _firebaseStorageService.DownloadFileAsync(fileName, folderName);

                if (fileStream == null)
                {
                    return NotFound();
                }

                // Trả về file cho người dùng (dạng PDF)
                return File(fileStream, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                // Xử lý trường hợp lỗi (nếu có)
                return StatusCode(500, new { message = "Error downloading the contract.", error = ex.Message });
            }
        }                   


        /// <summary>
        /// Update Contracts with all role.
        /// </summary>
        /// <returns>Update Contracts with all role.</returns>
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
