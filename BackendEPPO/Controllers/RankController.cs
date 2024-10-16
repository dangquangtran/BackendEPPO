using AutoMapper;
using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Rank;
using DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private IRankService _rankService;
        public RankController(IRankService rankService)
        {
            _rankService = rankService;
        }

      //  [Authorize(Roles = "admin, manager, staff")]
        [HttpGet(ApiEndPointConstant.Rank.GetListRank_Endpoint)]
        public async Task<IActionResult> GetListRanks(int page, int size)
        {
            var ranks = await _rankService.GetListRanks(page, size);

            if (ranks == null || !ranks.Any())
            {
                return NotFound("No ranks found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = ranks
            });
        }

        [Authorize(Roles = "admin, manager, staff, owner, customer")]
        [HttpGet(ApiEndPointConstant.Rank.GetRoleByID)]
        public async Task<IActionResult> GetRankByID(int id)
        {
            var ranks = await _rankService.GetRankByID(id);

            if (ranks == null)
            {
                return NotFound($"Rank with ID {id} not found.");
            }
            return Ok(new
            {
                StatusCode = 200,
                Message = "Request was successful",
                Data = ranks
            });
        }
        [Authorize(Roles = "admin, manager")]
        [HttpPost(ApiEndPointConstant.Rank.CreateRankByManager)]
        public async Task<IActionResult> CreateRankByManager([FromBody] CreateRankDTO rank)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _rankService.CreateRankByManager(rank);

            return Ok(new
            {
                StatusCode = 201,
                Message = "Rank created successfully",
                Data = rank
            });
        }

        [Authorize(Roles = "admin, manager, staff")]
        [HttpPut(ApiEndPointConstant.Rank.UpdateRank)]
        public async Task<IActionResult> UpdateRank(int id, [FromBody] UpdateRanksDTO rank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input data." });
            }
            rank.RankId = id;

            try
            {
                await _rankService.UpdateRank(rank);
                var updatedRank = await _rankService.GetRankByID(id);

                return Ok(new
                {
                    StatusCode = 201,
                    Message = "Rank updated successfully.",
                    Data = updatedRank
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Rank not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }
   
    }
}
