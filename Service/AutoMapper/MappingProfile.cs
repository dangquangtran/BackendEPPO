using AutoMapper;
using BusinessObjects.Models;
using DTOs.Conversation;
using DTOs.ImagePlant;
using DTOs.Message;
using DTOs.Order;
using DTOs.OrderDetail;
using DTOs.Plant;
using DTOs.Rank;
using DTOs.Services;
using DTOs.SubOrderDetail;
using DTOs.Transaction;
using DTOs.User;
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
            CreateMap<ChatMessageDTO, Message>();
            CreateMap<UpdateMessageDTO, Message>();
            CreateMap<Conversation, ConversationVM>().ReverseMap();
            CreateMap<User, UserVM>().ReverseMap();
            CreateMap<CreateTransactionDTO, Transaction>();
            CreateMap<UpdateTransactionDTO, Transaction>();
            CreateMap<Transaction, TransactionVM>().ReverseMap();
            CreateMap<CreatePlantDTO, Plant>();
            CreateMap<UpdatePlantDTO, Plant>();
            CreateMap<Plant, PlantVM>().ReverseMap();
            CreateMap<CreateOrderDTO, Order>();
            CreateMap<UpdateOrderDTO, Order>();
            CreateMap<Order, OrderVM>().ReverseMap();
            CreateMap<CreateOrderDetailDTO, OrderDetail>();
            CreateMap<CreateSubOrderDetailDTO, SubOrderDetail>();
            CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
            CreateMap<SubOrderDetail, SubOrderDetailVM>().ReverseMap();
            CreateMap<ImagePlant, ImagePlantVM>().ReverseMap();
            //Do Huu Thuan
            CreateMap<Plant, ResponsePlantDTO>().ReverseMap();
            CreateMap<User, ResponseUserDTO>().ReverseMap();
            CreateMap<BusinessObjects.Models.Epposervice, ServicesDTO>().ReverseMap();
        }

    }
}
