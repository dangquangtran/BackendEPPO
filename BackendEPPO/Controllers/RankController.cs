using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RankController : ControllerBase
    {
        private RankService _rankService;
        public RankController(RankService rankService)
        {
            _rankService = rankService;
        }

        [HttpGet]
        [Route("GetAllRanks")]
        public IActionResult GetAllRanks()
        {
            return Ok(_rankService.GetAllRanks());
        }
    }
}
