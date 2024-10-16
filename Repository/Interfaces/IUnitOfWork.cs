﻿using BusinessObjects.Models;
using Repository.Implements;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Rank> RankRepository { get; }


        //Do Huu Thuan
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Contract> ContractRepository { get; }
        IGenericRepository<Plant> PlantRepository { get; }
        IGenericRepository<Accessory> AccessoryRepository { get; }
        IGenericRepository<Epposervice> EpposerviceRepository { get; }
        IGenericRepository<Category> CategoriesRepository { get; }
        IGenericRepository<Address> AddressRepository { get; }
        IGenericRepository<ContractDetail> ContractDetailRepository { get; }
        IGenericRepository<Room> RoomRepository { get; }
        IGenericRepository<RoomParticipant> RoomParticipantRepository { get; }
        IGenericRepository<Notification> NotificationRepository { get; }
        IGenericRepository<Wallet> WalletRepository { get; }
        IGenericRepository<TypeEcommerce> TypeEcommerceRepository { get; }
        IGenericRepository<Blog> BlogRepository { get; }
        IGenericRepository<Feedback> FeedbackRepository { get; }
        IGenericRepository<SubFeedback> SubFeedbackRepository { get; }
        IGenericRepository<ImageFeedback> ImageFeedbackRepository { get; }
        IGenericRepository<UserVoucher> UserVoucherRepository { get; }
        

        Task SaveAsync(); 

        IGenericRepository<Transaction> TransactionRepository { get; }
        IGenericRepository<Conversation> ConversationRepository { get; }
        IGenericRepository<Message> MessageRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IGenericRepository<SubOrderDetail> SubOrderDetailRepository { get; }
        void Save();
    }
}
