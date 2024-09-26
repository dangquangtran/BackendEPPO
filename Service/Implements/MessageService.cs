using AutoMapper;
using BusinessObjects.Models;
using DTOs.Message;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class MessageService : IMessageService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public MessageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<Message> GetAllMessages()
        {
            return _unitOfWork.MessageRepository.Get();
        }

        public Message GetMessageById(int id)
        {
            return _unitOfWork.MessageRepository.GetByID(id);
        }

        public void CreateMessage(ChatMessageDTO createMessage)
        {
            Message message = _mapper.Map<Message>(createMessage);
            message.CreationDate = DateTime.Now;
            _unitOfWork.MessageRepository.Insert(message);
            _unitOfWork.Save();
        }
        public void UpdateMessage(UpdateMessageDTO updateMessage)
        {
            Message message = _mapper.Map<Message>(updateMessage);
            message.UpdateDate = DateTime.Now;
            _unitOfWork.MessageRepository.Update(message);
            _unitOfWork.Save();
        }

        public void DeleteMessage(int id)
        {
            _unitOfWork.MessageRepository.Delete(id);
            _unitOfWork.Save();
        }
    }
}
