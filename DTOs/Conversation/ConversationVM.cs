using DTOs.Message;
using DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.Conversation
{
    public class ConversationVM
    {
        public int ConversationId { get; set; }
        public int? UserOne { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? Status { get; set; }
        public UserVM UserOneNavigation { get; set; }
        public virtual ICollection<MessageVM> Messages { get; set; }
    }
}
