using BusinessObjects.Models;
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
        
        Task SaveAsync();

        IConversationRepository ConversationRepository { get; }
        IGenericRepository<Message> MessageRepository { get; }
        void Save();
    }
}
