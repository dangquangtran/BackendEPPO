using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Message
{
    public class MessageVM
    {
        public int ConversationId { get; set; }
        public int? UserId { get; set; }
        public string Message1 { get; set; }
        public string Type { get; set; }
        public string ImageLink { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsSeen { get; set; }
        public int? Status { get; set; }
    }
}
