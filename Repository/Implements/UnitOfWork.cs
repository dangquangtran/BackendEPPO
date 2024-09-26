using BusinessObjects.Models;
using Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly bef4qvhxkgrn0oa7ipg0Context context;
        private IGenericRepository<Rank> rankRepository;
        private IGenericRepository<User> userRepository;
        private IGenericRepository<Contract> contractRepository;
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
