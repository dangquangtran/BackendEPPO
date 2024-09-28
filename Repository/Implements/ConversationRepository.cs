using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Implements
{
    public class ConversationRepository : GenericRepository<Conversation>, IConversationRepository
    {
        public ConversationRepository(bef4qvhxkgrn0oa7ipg0Context context) : base(context)
        {
        }

        public IEnumerable<Conversation> GetConversationsByUserId(int userId)
        {
            return dbSet.Where(c => c.UserOne == userId || c.UserTwo ==userId).Include(c => c.Messages.OrderByDescending(m => m.CreationDate)).Include(c => c.UserOneNavigation).Include(c=> c.UserTwoNavigation).ToList();
        }
    }
}
