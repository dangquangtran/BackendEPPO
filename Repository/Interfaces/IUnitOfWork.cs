using BusinessObjects.Models;
using System;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Rank> RankRepository { get; }

        IGenericRepository<User> UserRepository { get; }
        Task SaveAsync();

        void Save();
    }
}
