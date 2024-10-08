using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Message
{
    public class ChatMessageDTO
    {
        public int ConversationId { get; set; }
        public string Message1 { get; set; }
        public string Type { get; set; }
        public string ImageLink { get; set; }
    }
}
