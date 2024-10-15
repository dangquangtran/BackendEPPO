using AutoMapper;
using BackendEPPO.Extenstion;
using BusinessObjects.Models;
using DTOs.Rank;
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

        [HttpPost("Ranks")]
        public IActionResult CreateRank([FromBody] CreateRankDTO createRank)
        {
            _rankService.CreateRank(createRank);
            return Ok();
        }

        [HttpPut("Ranks")]
        public IActionResult UpdateRank([FromBody] UpdateRankDTO updateRank)
        {
            _rankService.UpdateRank(updateRank);
            return Ok();
        }
    }
}
