using DTOs.Conversation;
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

        [HttpGet("Conversations")]
        public IActionResult GetAllConversations()
        {
            return Ok(_conversationService.GetAllConversations());
        }

        [HttpGet("Conversations/{id}")]
        public IActionResult GetConversationById(int id)
        {
            return Ok(_conversationService.GetConversationById(id));
        }

        [HttpPost("Conversations")]
        public IActionResult CreateConversation([FromBody] CreateConversationDTO createConversation)
        {
            _conversationService.CreateConversation(createConversation);
            return Ok("Đã tạo thành công");
        }

        [HttpPut("Conversations")]
        public IActionResult UpdateConversation([FromBody] UpdateConversationDTO updateConversation)
        {
            _conversationService.UpdateConversation(updateConversation);
            return Ok("Đã cập nhật thành công");
        }

        [HttpDelete("Conversations/{id}")]
        public IActionResult DeleteConversation(int id)
        {
            _conversationService.DeleteConversation(id);
            return Ok("Đã xóa thành công");
        }

        [HttpGet("Conversations/GetByUserId")]
        public IActionResult GetConversationsByUserId([FromQuery] int userId)
        {
            return Ok(_conversationService.GetConversationsByUserId(userId));
        }
    }
}
