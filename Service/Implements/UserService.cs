using AutoMapper;
using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.User;
using Microsoft.AspNetCore.Http;
using Mysqlx.Crud;
using Repository.Interfaces;
using Service.Implements;
using Service.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly FirebaseStorageService _firebaseStorageService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, FirebaseStorageService firebaseStorageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _firebaseStorageService = firebaseStorageService;
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
            var walletEntity = new Wallet
            {
                NumberBalance = 0,              
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Status = 1,
            };

            _unitOfWork.WalletRepository.Insert(walletEntity);
            await _unitOfWork.SaveAsync();

            var customerEntity = new User
            {
                UserName= customer.UserName,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Password = customer.Password,
                WalletId = walletEntity.WalletId,
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
            var walletEntity = new Wallet
            {
                NumberBalance = 0,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Status = 1,
            };

            var customerEntity = new User
            {
                UserName = owner.UserName,
                FullName = owner.FullName,
                PhoneNumber = owner.PhoneNumber,
                Email = owner.Email,
                Password = owner.Password,
                WalletId = walletEntity.WalletId,
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
            var walletEntity = new Wallet
            {
                NumberBalance = 0,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Status = 1,
            };

            var customerEntity = new User
            {
                UserName = admin.UserName,
                FullName = admin.FullName,
                PhoneNumber = admin.PhoneNumber,
                Email = admin.Email,
                Password = admin.Password,
                WalletId = walletEntity.WalletId,
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
            return await Task.FromResult(_unitOfWork.UserRepository.GetByID(Id, includeProperties: "Wallet"));
        }

        public async Task UpdateUserAccount(UpdateAccount accountDTO, IFormFile imageFile)
        {
            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(accountDTO.UserId));

            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found.");
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

            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                string fileName = imageFile.FileName;

                string newImageUrl = await _firebaseStorageService.UploadUserImageAsync(stream, fileName);

                if (!string.IsNullOrWhiteSpace(userEntity.ImageUrl))
                {
                    //await _firebaseStorageService.DeleteUserImageAsync(userEntity.ImageUrl);
                }

                // Cập nhật URL ảnh mới vào userEntity
                userEntity.ImageUrl = newImageUrl;
            }

            _unitOfWork.UserRepository.Update(userEntity);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateInformationAccount(UpdateInformation accountDTO, IFormFile imageFile)
        {
            var entity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(accountDTO.UserId));

            if (entity == null)
            {
                throw new Exception($"Contract with ID {accountDTO.UserId} not found.");
            }
            entity.FullName = accountDTO.FullName;
            entity.Gender = accountDTO.Gender;
            entity.DateOfBirth = accountDTO.DateOfBirth;
            entity.PhoneNumber = accountDTO.PhoneNumber;
            entity.Email = accountDTO.Email;
            entity.ImageUrl = accountDTO.ImageUrl;
            entity.IdentificationCard = accountDTO.IdentificationCard;


            if (imageFile != null)
            {
                using var stream = imageFile.OpenReadStream();
                string fileName = imageFile.FileName;

                string newImageUrl = await _firebaseStorageService.UploadUserImageAsync(stream, fileName);

                if (!string.IsNullOrWhiteSpace(accountDTO.ImageUrl))
                {
                    //await _firebaseStorageService.DeleteUserImageAsync(userEntity.ImageUrl);
                }

                // Cập nhật URL ảnh mới vào userEntity
                entity.ImageUrl = newImageUrl;
            }


            _unitOfWork.UserRepository.Update(entity);
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

        public User GetUserByID(int id)
        {
            return _unitOfWork.UserRepository.GetByID(id);
        }
    }
}
