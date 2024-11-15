using AutoMapper;
using BusinessObjects.Models;
using DTOs.Conversation;
using DTOs.ImagePlant;
using DTOs.Message;
using DTOs.Order;
using DTOs.OrderDetail;
using DTOs.Plant;
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
            CreateMap<Order, OrderRentalVM>().ReverseMap();
            CreateMap<CreateOrderDetailDTO, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
            CreateMap<OrderDetail, OrderDetailRentalVM>().ReverseMap();
            CreateMap<CreateOrderRentalDTO, Order>();
            CreateMap<CreateOrderDetailRentalDTO, OrderDetail>();
            CreateMap<OrderDetail, OrderDetailRentalVM>().ReverseMap();
            CreateMap<ImagePlant, ImagePlantVM>().ReverseMap();
            //Do Huu Thuan
            CreateMap<Plant, ResponsePlantDTO>().ReverseMap();
            CreateMap<User, ResponseUserDTO>().ReverseMap();
        }

    }
}
