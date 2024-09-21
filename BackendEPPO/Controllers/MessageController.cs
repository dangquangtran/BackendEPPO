using DTOs.Message;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;

namespace BackendEPPO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IMessageService _messageService;
        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet("Messages")]
        public IActionResult GetAllMessages()
        {
            return Ok(_messageService.GetAllMessages());
        }

        [HttpGet("Messages/{id}")]
        public IActionResult GetMessageById(int id)
        {
            return Ok(_messageService.GetMessageById(id));
        }

        [HttpPost("Messages")]
        public IActionResult CreateMessage([FromBody] CreateMessageDTO createMessage)
        {
            _messageService.CreateMessage(createMessage);
            return Ok();
        }

        [HttpPut("Messages")]
        public IActionResult UpdateMessage([FromBody] UpdateMessageDTO updateMessage)
        {
            _messageService.UpdateMessage(updateMessage);
            return Ok();
        }

        [HttpDelete("Messages/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            _messageService.DeleteMessage(id);
            return Ok();
        }
    }
}
