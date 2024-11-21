using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.User;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetListUsers(int page, int size);
        Task<IEnumerable<User>> FilterAccountByRoleID(int page, int size, int roleId);
        Task<IEnumerable<User>> SearchAccountByKey(int page, int size, string keyWork);
        Task<User> GetUsersByID(int Id);
        //Task<User> GetInformationByID(int Id);
        IQueryable<User> GetAllUsers();
        Task CreateUserAccount(ResponseUserDTO user);
        Task CreateAccountByCustomer(CreateAccountByCustomerDTO customer);
        Task CreateAccountByOwner(CreateAccountByOwnerDTO owner);
        Task CreateAccountByAdmin(CreateAccountByAdminDTO admin);
        User GetUserByID(int id);

        Task UpdateUserAccount(UpdateAccount account, IFormFile imageFile);
        Task UpdateInformationAccount(UpdateInformation account, IFormFile imageFile , int useID);
        Task ChangePasswordAccount(ChangePassword account);

        Task<bool> CheckAccountExists(string email, string userName);
        Task<int> CountAccountByStatus(int status);
        Task UpdateRankVler(UpdateRankVler account);
        //Task<bool> UpdateUserIsSignedAsync(int userId, bool isSigned);
    }
}
