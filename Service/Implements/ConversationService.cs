using AutoMapper;
using BusinessObjects.Models;
using DTOs.Conversation;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class ConversationService : IConversationService
    {
        private IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ConversationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<ConversationVM> GetAllConversations()
        {
            var conversations = _unitOfWork.ConversationRepository.Get(filter: c => c.Status != 0, includeProperties: "Messages,UserOneNavigation,UserTwoNavigation");
            return _mapper.Map<IEnumerable<ConversationVM>>(conversations);
        }

        public Conversation GetConversationById(int id)
        {
            return _unitOfWork.ConversationRepository.GetByID(id);
        }

        public void CreateConversation(CreateConversationDTO createConversation)
        {
            Conversation conversation = _mapper.Map<Conversation>(createConversation);
            conversation.CreationDate = DateTime.Now;
            conversation.Status = 1;
            _unitOfWork.ConversationRepository.Insert(conversation);
            _unitOfWork.Save();
        }
        public void UpdateConversation(UpdateConversationDTO updateConversation)
        {
            Conversation conversation = _mapper.Map<Conversation>(updateConversation);
            _unitOfWork.ConversationRepository.Update(conversation);
            _unitOfWork.Save();
        }

        public IEnumerable<ConversationVM> GetConversationsByUserId(int userId)
        {
            var conversations = _unitOfWork.ConversationRepository.Get(
        filter: c => (c.UserOne == userId || c.UserTwo == userId) && c.Status != 0,
        includeProperties: "Messages,UserOneNavigation,UserTwoNavigation"
        );
            foreach (var conversation in conversations)
            {
                conversation.Messages = conversation.Messages.OrderBy(m => m.CreationDate).ToList();
            }
            return _mapper.Map<IEnumerable<ConversationVM>>(conversations);
        }
    }
}
