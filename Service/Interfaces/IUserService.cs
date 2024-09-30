using BusinessObjects.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetListUsers();
        Task<User> GetUsersByID(int Id);
    }
}
