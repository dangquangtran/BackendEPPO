using BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetListUsers();
    }
}
