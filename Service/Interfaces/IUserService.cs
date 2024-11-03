using BusinessObjects.Models;
using DTOs.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetListUsers(int page, int size);
        Task<User> GetUsersByID(int Id);
        IQueryable<User> GetAllUsers();


        Task CreateUserAccount(ResponseUserDTO user);
        User GetUserByID(int id);

    }
}
