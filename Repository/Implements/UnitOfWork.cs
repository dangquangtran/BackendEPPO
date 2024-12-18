﻿using BusinessObjects.Models;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly bef4qvhxkgrn0oa7ipg0Context context;
        //Do Huu Thuan
        private IGenericRepository<User> userRepository;
        private IGenericRepository<Contract> contractRepository;
        private IGenericRepository<Plant> plantRepository;
        private IGenericRepository<Category> categoryRepository;
        private IGenericRepository<Address> addressRepository;
        private IGenericRepository<ContractDetail> contractDetailRepository;
        private IGenericRepository<Room> roomRepository;
        private IGenericRepository<Notification> notificationtRepository;
        private IGenericRepository<Wallet> walletRepository;
        private IGenericRepository<TypeEcommerce> typeEcommerceRepository;
        private IGenericRepository<Feedback> feedbackRepository;
        private IGenericRepository<ImageFeedback> imageFeedbackRepository;
        private IGenericRepository<UserRoom> userRoomRepository;

        private IGenericRepository<Transaction> transactionRepository;
        private IGenericRepository<Conversation> conversationRepository;
        private IGenericRepository<Message> messageRepository;
        private IGenericRepository<Order> orderRepository;
        private IGenericRepository<OrderDetail> orderDetailRepository;
        private IGenericRepository<HistoryBid>historyBidRepository;

        public UnitOfWork(bef4qvhxkgrn0oa7ipg0Context context)
        {
            this.context = context;
        }
        //Do Huu Thuan
        public IGenericRepository<UserRoom> UserRoomRepository
        {
            get
            {
                return userRoomRepository ??= new GenericRepository<UserRoom>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<ImageFeedback> ImageFeedbackRepository
        {
            get
            {
                return imageFeedbackRepository ??= new GenericRepository<ImageFeedback>(context);
            }
        }
       
        //Do Huu Thuan
        public IGenericRepository<Feedback> FeedbackRepository
        {
            get
            {
                return feedbackRepository ??= new GenericRepository<Feedback>(context);
            }
        }        
        //Do Huu Thuan
        public IGenericRepository<TypeEcommerce> TypeEcommerceRepository
        {
            get
            {
                return typeEcommerceRepository ??= new GenericRepository<TypeEcommerce>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Wallet> WalletRepository
        {
            get
            {
                return walletRepository ??= new GenericRepository<Wallet>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Room> RoomRepository
        {
            get
            {
                return roomRepository ??= new GenericRepository<Room>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Notification> NotificationRepository
        {
            get
            {
                return notificationtRepository ??= new GenericRepository<Notification>(context);
            }
        }
        //Do Huu Thuan
        
        public IGenericRepository<ContractDetail> ContractDetailRepository
        {
            get
            {
                return contractDetailRepository ??= new GenericRepository<ContractDetail>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Address> AddressRepository
        {
            get
            {
                return addressRepository ??= new GenericRepository<Address>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<User> UserRepository
        {
            get
            {
                return userRepository ??= new GenericRepository<User>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Contract> ContractRepository
        {
            get
            {
                return contractRepository ??= new GenericRepository<Contract>(context);
            }
        }    
        //Do Huu Thuan
        public IGenericRepository<Plant> PlantRepository
        {
            get
            {
                return plantRepository ??= new GenericRepository<Plant>(context);
            }
        }
       
        public IGenericRepository<Category> CategoriesRepository
        {
            get
            {
                return categoryRepository ??= new GenericRepository<Category>(context);
            }
        }
        public IGenericRepository<Conversation> ConversationRepository
        {
            get
            {
                return conversationRepository ??= new GenericRepository<Conversation>(context);
            }
        }

        public IGenericRepository<Message> MessageRepository
        {
            get
            {
                return messageRepository ??= new GenericRepository<Message>(context);
            }
        }

        public IGenericRepository<Transaction> TransactionRepository
        {
            get
            {
                return transactionRepository ??= new GenericRepository<Transaction>(context);
            }
        }

        public IGenericRepository<Order> OrderRepository
        {
            get
            {
                return orderRepository ??= new GenericRepository<Order>(context);
            }
        }

        public IGenericRepository<OrderDetail> OrderDetailRepository
        {
            get
            {
                return orderDetailRepository ??= new GenericRepository<OrderDetail>(context);
            }
        }

        public IGenericRepository<HistoryBid> HistoryBidRepository
        {
            get
            {
                return historyBidRepository ??= new GenericRepository<HistoryBid>(context);
            }
        }


        public void Save()
        {
            context.SaveChanges();
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
