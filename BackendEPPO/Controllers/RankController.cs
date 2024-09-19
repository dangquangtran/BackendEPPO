using AutoMapper;
using BusinessObjects.Models;
using DTOs.Rank;
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

        [HttpGet("Ranks")]
        public IActionResult GetAllRanks()
        {
            return Ok(_rankService.GetAllRanks());
        }

        [HttpGet("Ranks/{id}")]
        public IActionResult GetRankById(int id)
        {
            return Ok(_rankService.GetRankById(id));
        }

        [HttpPost("Ranks")]
        public IActionResult CreateRank([FromBody] CreateRank createRank)
        {
            _rankService.CreateRank(createRank);
            return Ok();
        }

        [HttpPut("Ranks")]
        public IActionResult UpdateRank([FromBody] UpdateRank updateRank)
        {
            _rankService.UpdateRank(updateRank);
            return Ok();
        }

        [HttpDelete("Ranks/{id}")]
        public IActionResult DeleteRank(int id)
        {
            _rankService.DeleteRank(id);
            return Ok();
        }

    }
}
