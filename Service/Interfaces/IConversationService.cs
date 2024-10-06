using BusinessObjects.Models;
using DTOs.Conversation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IConversationService
    {
        IEnumerable<ConversationVM> GetAllConversations();
        Conversation GetConversationById(int id);
        void CreateConversation(CreateConversationDTO createConversation);
        void UpdateConversation(UpdateConversationDTO updateConversation);
        IEnumerable<ConversationVM> GetConversationsByUserId(int userId);
    }
}
