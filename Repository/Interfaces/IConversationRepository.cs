using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Interfaces
{
    public interface IConversationRepository : IGenericRepository<Conversation>
    {
        IEnumerable<Conversation> GetConversationsByUserId(int userId);
    }
}
