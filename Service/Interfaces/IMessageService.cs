using BusinessObjects.Models;
using DTOs.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IMessageService
    {
        IEnumerable<Message> GetAllMessages();
        Message GetMessageById(int id);
        void CreateMessage(ChatMessageDTO createMessage);
        void UpdateMessage(UpdateMessageDTO updateMessage);
        void DeleteMessage(int id);
    }
}
