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

        public IQueryable<User> GetAllUsers()
        {
            return _unitOfWork.UserRepository.Get(includeProperties: "Role").AsQueryable(); 
        }

        public async Task<IEnumerable<User>> GetListUsers(int page, int size)
        {
            return await _unitOfWork.UserRepository.GetAsync(pageIndex: page, pageSize: size); 
        }
        public async Task<User> GetUsersByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserRepository.GetByID(Id));
        }
    }
}
