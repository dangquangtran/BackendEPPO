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
        private IGenericRepository<Accessory> accessoryRepository;
        private IGenericRepository<Service> serviceRepository;
        private IGenericRepository<Category> categoryRepository;
        private IGenericRepository<Address> addressRepository;
        private IGenericRepository<ContractDetail> contractDetailRepository;
        private IGenericRepository<Room> roomRepository;
        private IGenericRepository<RoomParticipant> roomParticipantRepository;
        private IGenericRepository<Notification> notificationtRepository;

        private IGenericRepository<Rank> rankRepository;
   

        private IConversationRepository conversationRepository;
        private IGenericRepository<Message> messageRepository;

        public UnitOfWork(bef4qvhxkgrn0oa7ipg0Context context)
        {
            this.context = context;
        }

        public IGenericRepository<Rank> RankRepository
        {
            get
            {
                return rankRepository ??= new GenericRepository<Rank>(context);
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
        public IGenericRepository<RoomParticipant> RoomParticipantRepository
        {
            get
            {
                return roomParticipantRepository ??= new GenericRepository<RoomParticipant>(context);
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
        //Do Huu Thuan
        public IGenericRepository<Accessory> AccessoryRepository
        {
            get
            {
                return accessoryRepository ??= new GenericRepository<Accessory>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Service> ServicesRepository
        {
            get
            {
                return serviceRepository ??= new GenericRepository<Service>(context);
            }
        }
        //Do Huu Thuan
        public IGenericRepository<Category> CategoriesRepository
        {
            get
            {
                return categoryRepository ??= new GenericRepository<Category>(context);
            }
        }
        public IConversationRepository ConversationRepository
        {
            get
            {
                return conversationRepository ??= new ConversationRepository(context);
            }
        }

        public IGenericRepository<Message> MessageRepository
        {
            get
            {
                return messageRepository ??= new GenericRepository<Message>(context);
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
