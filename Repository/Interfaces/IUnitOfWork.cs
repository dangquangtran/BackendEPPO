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
        Task SaveAsync();

        void Save();
    }
}
