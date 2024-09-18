using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public int? ConversationId { get; set; }
        public int? UserId { get; set; }
        public string Message1 { get; set; }
        public string Type { get; set; }
        public string ImageLink { get; set; }
        public DateTime? CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsSent { get; set; }
        public bool? IsSeen { get; set; }
        public int? Status { get; set; }

        public virtual Conversation Conversation { get; set; }
        public virtual User User { get; set; }
    }
}
