using DTOs.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet]
        public IActionResult GetAllMessages()
        {
            return Ok(_messageService.GetAllMessages());
        }

        [HttpGet("{id}")]
        public IActionResult GetMessageById(int id)
        {
            return Ok(_messageService.GetMessageById(id));
        }

        [HttpPost]
        public IActionResult CreateMessage([FromBody] ChatMessageDTO createMessage)
        {
            _messageService.CreateMessage(createMessage);
            return Ok(("Đã tạo thành công"));
        }

        [HttpPut]
        public IActionResult UpdateMessage([FromBody] UpdateMessageDTO updateMessage)
        {
            _messageService.UpdateMessage(updateMessage);
            return Ok("Đã cập nhật thành công");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteMessage(int id)
        {
            _messageService.DeleteMessage(id);
            return Ok("Đã xóa thành công");
        }
    }
}
