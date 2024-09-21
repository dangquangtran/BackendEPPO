using AutoMapper;
using BusinessObjects.Models;
using DTOs.Conversation;
using DTOs.Message;
using DTOs.Rank;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateRankDTO, Rank>();
            CreateMap<UpdateRankDTO, Rank>();
            CreateMap<CreateConversationDTO, Conversation>();
            CreateMap<UpdateConversationDTO, Conversation>();
            CreateMap<CreateMessageDTO, Message>();
            CreateMap<UpdateMessageDTO, Message>();
        }

    }
}
