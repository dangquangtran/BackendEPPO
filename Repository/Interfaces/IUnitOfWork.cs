using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Rank> RankRepository { get; }

        //Do Huu Thuan
        IGenericRepository<User> UserRepository { get; }

        void Save();
    }
}
