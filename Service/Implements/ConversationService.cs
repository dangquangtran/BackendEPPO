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
        public IEnumerable<Conversation> GetAllConversations()
        {
            return _unitOfWork.ConversationRepository.Get();
        }

        public Conversation GetConversationById(int id)
        {
            return _unitOfWork.ConversationRepository.GetByID(id);
        }

        public void CreateConversation(CreateConversationDTO createConversation)
        {
            Conversation conversation = _mapper.Map<Conversation>(createConversation);
            conversation.CreationDate = DateTime.Now;
            _unitOfWork.ConversationRepository.Insert(conversation);
            _unitOfWork.Save();
        }
        public void UpdateConversation(UpdateConversationDTO updateConversation)
        {
            Conversation conversation = _mapper.Map<Conversation>(updateConversation);
            _unitOfWork.ConversationRepository.Update(conversation);
            _unitOfWork.Save();
        }

        public void DeleteConversation(int id)
        {
            _unitOfWork.ConversationRepository.Delete(id);
            _unitOfWork.Save();
        }
    }
}
