using BackendEPPO.Extenstion;
using DTOs.Conversation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConversationController : ControllerBase
    {
        private IConversationService _conversationService;
        public ConversationController(IConversationService conversationService)
        {
            _conversationService = conversationService;
        }

        [HttpGet]
        public IActionResult GetAllConversations()
        {
            return Ok(_conversationService.GetAllConversations());
        }

        [HttpGet("{id}")]
        public IActionResult GetConversationById(int id)
        {
            return Ok(_conversationService.GetConversationById(id));
        }

        [HttpPost]
        public IActionResult CreateConversation([FromBody] CreateConversationDTO createConversation)
        {
            _conversationService.CreateConversation(createConversation);
            return Ok("Đã tạo thành công");
        }

        [HttpPut]
        public IActionResult UpdateConversation([FromBody] UpdateConversationDTO updateConversation)
        {
            _conversationService.UpdateConversation(updateConversation);
            return Ok("Đã cập nhật thành công");
        }

        [Authorize]
        [HttpGet("GetByUserId")]
        public IActionResult GetConversationsByUserId()
        {
            var userIdClaim =User.FindFirst("userId")?.Value;
            int userId = int.Parse(userIdClaim);
            return Ok(_conversationService.GetConversationsByUserId(userId));
        }
    }
}
