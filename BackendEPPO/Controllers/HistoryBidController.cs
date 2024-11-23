using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryBidController : ControllerBase
    {
        private readonly IHistoryBidService _historyBidService;

        public HistoryBidController(IHistoryBidService historyBidService)
        {
            _historyBidService = historyBidService;
        }

        [HttpGet("GetHistoryBidsByRoomId")]
        public IActionResult GetHistoryBidsByRoomId(int pageIndex, int pageSize, int roomId)
        {
            try
            {
                var result = _historyBidService.GetHistoryBidsByRoomId(pageIndex, pageSize, roomId);
                if (result == null || !result.Any())
                {
                    return NotFound(new
                    {
                        StatusCode = 404,
                        Message = $"Không tìm thấy lịch sử đặt cược nào cho Room ID {roomId}.",
                        Data = (object)null
                    });
                }
                return Ok(new
                {
                    StatusCode = 200,
                    Message = "Yêu cầu thành công.",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    StatusCode = 400,
                    Message = "Có lỗi xảy ra: " + ex.Message,
                    Data = (object)null
                });
            }
        }
    }
}
