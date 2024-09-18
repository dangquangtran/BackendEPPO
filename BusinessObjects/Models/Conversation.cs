using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class Conversation
    {
        public Conversation()
        {
            Messages = new HashSet<Message>();
        }

        public int ConversationId { get; set; }
        public int? UserOne { get; set; }
        public int? UserTwo { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Status { get; set; }

        public virtual User UserOneNavigation { get; set; }
        public virtual User UserTwoNavigation { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
