using BusinessObjects.Models;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<User>> GetListUsers()
        {
            return await _unitOfWork.UserRepository.GetAsync(); 
        }
        public async Task<User> GetUsersByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserRepository.GetByID(Id));
        }
    }
}
