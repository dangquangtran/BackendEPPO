using BusinessObjects.Models;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly bef4qvhxkgrn0oa7ipg0Context context;
        private IGenericRepository<Rank> rankRepository;
        private IGenericRepository<Conversation> conversationRepository;
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

        public void Save()
        {
            context.SaveChanges();
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
