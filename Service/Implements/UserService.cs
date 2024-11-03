using AutoMapper;
using BusinessObjects.Models;
using DTOs.User;
using Repository.Interfaces;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task CreateUserAccount(ResponseUserDTO userDto)
        {
            var userEntity = new User
            {
                UserName = userDto.UserName,
                Password = userDto.Password,
                PhoneNumber = userDto.PhoneNumber,
                Email = userDto.Email,
                RoleId = 4,
                CreationDate = DateTime.Now,
                IsActive = true,
                Status = 1,

            };

            _unitOfWork.UserRepository.Insert(userEntity); 
            await _unitOfWork.SaveAsync();
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

        public User GetUserByID(int id)
        {
            return _unitOfWork.UserRepository.GetByID(id);
        }
    }
}
