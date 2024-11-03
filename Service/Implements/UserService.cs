using AutoMapper;
using BusinessObjects.Models;
using DTOs.User;
using Mysqlx.Crud;
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
        public async Task CreateAccountByCustomer(CreateAccountByCustomerDTO customer)
        {
            var customerEntity = new User
            {
                UserName= customer.UserName,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Password = customer.Password,
                RoleId = 5,
                CreationDate = DateTime.Now,
                IsActive = true,
                Status = 1,

            };

            _unitOfWork.UserRepository.Insert(customerEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateAccountByOwner(CreateAccountByOwnerDTO owner)
        {
            var customerEntity = new User
            {
                UserName = owner.UserName,
                FullName = owner.FullName,
                PhoneNumber = owner.PhoneNumber,
                Email = owner.Email,
                Password = owner.Password,
                RoleId = 4,
                CreationDate = DateTime.Now,
                IsActive = true,
                Status = 1,

            };

            _unitOfWork.UserRepository.Insert(customerEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task CreateAccountByAdmin(CreateAccountByAdminDTO admin)
        {
            var customerEntity = new User
            {
                UserName = admin.UserName,
                FullName = admin.FullName,
                PhoneNumber = admin.PhoneNumber,
                Email = admin.Email,
                Password = admin.Password,
                RoleId = 3,
                CreationDate = DateTime.Now,
                IsActive = true,
                Status = 1,

            };

            _unitOfWork.UserRepository.Insert(customerEntity);
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

        public async Task UpdateUserAccount(UpdateAccount accountDTO)
        {
            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(accountDTO.UserId));

            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.UserName))
            {
                userEntity.UserName = accountDTO.UserName;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.Password))
            {
                userEntity.Password = accountDTO.Password; 
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.FullName))
            {
                userEntity.FullName = accountDTO.FullName;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.Gender))
            {
                userEntity.Gender = accountDTO.Gender;
            }
            if (accountDTO.DateOfBirth.HasValue)
            {
                userEntity.DateOfBirth = accountDTO.DateOfBirth.Value;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.PhoneNumber))
            {
                userEntity.PhoneNumber = accountDTO.PhoneNumber;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.Email))
            {
                userEntity.Email = accountDTO.Email;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.ImageUrl))
            {
                userEntity.ImageUrl = accountDTO.ImageUrl;
            }
            if (accountDTO.IdentificationCard.HasValue)
            {
                userEntity.IdentificationCard = accountDTO.IdentificationCard.Value;
            }
            if (accountDTO.WalletId.HasValue)
            {
                userEntity.WalletId = accountDTO.WalletId.Value;
            }

            if (accountDTO.RoleId.HasValue)
            {
                userEntity.RoleId = accountDTO.RoleId.Value;
            }

            if (accountDTO.IsActive.HasValue)
            {
                userEntity.IsActive = accountDTO.IsActive.Value;
            }

            if (accountDTO.CreationDate.HasValue)
            {
                userEntity.CreationDate = accountDTO.CreationDate.Value;
            }

            if (accountDTO.CreationBy.HasValue)
            {
                userEntity.CreationBy = accountDTO.CreationBy.Value;
            }

            userEntity.ModificationDate = DateTime.Now;
            userEntity.ModificationBy = accountDTO.ModificationBy;

            if (accountDTO.Status.HasValue)
            {
                userEntity.Status = accountDTO.Status.Value;
            }

            _unitOfWork.UserRepository.Update(userEntity);
            await _unitOfWork.SaveAsync();
        }
        public async Task<bool> CheckAccountExists(string email, string userName)
        {
            var existingUser = await _unitOfWork.UserRepository
                .GetFirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                           || u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            return existingUser != null;
        }

        public async Task ChangePasswordAccount(ChangePassword account)
        {
            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(account.UserId));
            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (string.IsNullOrWhiteSpace(account.Password))
            {
                throw new ArgumentException("Password not null.");
            }
            userEntity.Password = account.Password;
            userEntity.ModificationDate = DateTime.Now;
            _unitOfWork.UserRepository.Update(userEntity);
            await _unitOfWork.SaveAsync();
        }

    }
}
