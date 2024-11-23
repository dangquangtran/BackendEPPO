using AutoMapper;
using BusinessObjects.Models;
using DTOs.Contracts;
using DTOs.User;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Mysqlx.Crud;
using Repository.Interfaces;
using Service.Implements;
using Service.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
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
                UserName = customer.UserName,
                FullName = customer.FullName,
                PhoneNumber = customer.PhoneNumber,
                Gender = customer.Gender,
                DateOfBirth = customer.DateOfBirth,
                Email = customer.Email,
                IdentificationCard = customer.IdentificationCard,
                Password = customer.Password,
                WalletId = walletEntity.WalletId,
                RankLevel = "New account",
                IsSigned = false,
                IsUpdated = false,
                RoleId = 5,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                IsActive = true,
                Status = 1,
            };

            _unitOfWork.UserRepository.Insert(customerEntity);
            await _unitOfWork.SaveAsync();

            var addressEntity = new Address
            {
                UserId = customerEntity.UserId,
                Description = customer.AddressDescription,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Status = 1,
            };



            _unitOfWork.AddressRepository.Insert(addressEntity);
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

            _unitOfWork.WalletRepository.Insert(walletEntity);
            await _unitOfWork.SaveAsync();

            var customerEntity = new User
            {
                UserName = owner.UserName,
                FullName = owner.FullName,
                PhoneNumber = owner.PhoneNumber,
                Gender = owner.Gender,
                DateOfBirth = owner.DateOfBirth,
                Email = owner.Email,
                IdentificationCard = owner.IdentificationCard,
                Password = owner.Password,
                WalletId = walletEntity.WalletId,
                RankLevel = "New account",
                IsSigned =false,
                IsUpdated = false,
                RoleId = 4,
                CreationDate = DateTime.Now,
                ModificationDate= DateTime.Now,
                IsActive = true,
                Status = 1,
            };

            _unitOfWork.UserRepository.Insert(customerEntity);
            await _unitOfWork.SaveAsync();

            var addressEntity = new Address
            {
                UserId = customerEntity.UserId,
                Description = owner.AddressDescription,
                CreationDate = DateTime.Now,
                ModificationDate = DateTime.Now,
                Status = 1,
            };
          


            _unitOfWork.AddressRepository.Insert(addressEntity);
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
            return await _unitOfWork.UserRepository.GetAsync(pageIndex: page, orderBy: query => query.OrderByDescending(c => c.UserId), pageSize: size); 
        }

        public async Task<IEnumerable<User>> FilterAccountByRoleID(int page, int size, int roleId)
        {
            return await _unitOfWork.UserRepository.GetAsync(pageIndex: page, pageSize: size, orderBy: query => query.OrderByDescending(c => c.UserId), filter: u => u.RoleId == roleId);
        }
        public async Task<IEnumerable<User>> SearchAccountByKey(int page, int size, string keyWord)
        {
            return await _unitOfWork.UserRepository.GetAsync(
                pageIndex: page, 
                pageSize: size,
                orderBy: query => query.OrderByDescending(c => c.UserId),
                includeProperties: "Wallet",
                filter: u => 
                (u.FullName.Contains(keyWord) || u.Email.Contains(keyWord) || u.PhoneNumber.Contains(keyWord)));
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

            if (!accountDTO.DateOfBirth.HasValue)
            {
                userEntity.DateOfBirth = accountDTO.DateOfBirth.Value;
            }

            //if (!string.IsNullOrEmpty(accountDTO.DateOfBirthInput))
            //{
            //    if (DateTime.TryParseExact(
            //        accountDTO.DateOfBirthInput,
            //        "dd/MM/yy",
            //        CultureInfo.InvariantCulture,
            //        DateTimeStyles.None,
            //        out DateTime parsedDate))
            //    {
            //        userEntity.DateOfBirth = parsedDate; 
            //    }
            //    else
            //    {
            //        throw new FormatException("Invalid date format. Please use dd/MM/yy.");
            //    }
            //}


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


            if (!string.IsNullOrWhiteSpace(accountDTO.IdentificationCard))
            {
                userEntity.IdentificationCard = accountDTO.IdentificationCard;
            }

            //if (accountDTO.IdentificationCard.HasValue)
            //{
            //    userEntity.IdentificationCard = accountDTO.IdentificationCard.Value;
            //}
            if (accountDTO.WalletId.HasValue)
            {
                userEntity.WalletId = accountDTO.WalletId.Value;
            }
            if (accountDTO.IsActive.HasValue)
            {
                userEntity.IsActive = accountDTO.IsActive.Value;
            }
            if (accountDTO.IsUpdated.HasValue)
            {
                userEntity.IsUpdated = true;
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


        public async Task UpdateInformationAccount(UpdateInformation accountDTO, IFormFile imageFile, int useID)
        {

            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(useID));

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

            if (!accountDTO.DateOfBirth.HasValue)
            {
                userEntity.DateOfBirth = accountDTO.DateOfBirth.Value;
            }
            //if (!string.IsNullOrEmpty(accountDTO.DateOfBirthInput))
            //{
            //    if (DateTime.TryParseExact(
            //        accountDTO.DateOfBirthInput,
            //        "dd/MM/yyyy",
            //        CultureInfo.InvariantCulture,
            //        DateTimeStyles.None,
            //        out DateTime parsedDate))
            //    {
            //        userEntity.DateOfBirth = parsedDate;
            //    }
            //    else
            //    {
            //        throw new FormatException("Invalid date format. Please use dd/MM/yy.");
            //    }
            //}
            if (!string.IsNullOrWhiteSpace(accountDTO.PhoneNumber))
            {
                if (Regex.IsMatch(accountDTO.PhoneNumber, @"^\d{10}$"))
                {
                    userEntity.PhoneNumber = accountDTO.PhoneNumber;
                }
                else
                {
                    throw new ArgumentException("Số điện thoại không hợp lệ. Số điện thoại phải có 10 chữ số.");
                }
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.Email))
            {
                if (accountDTO.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    userEntity.Email = accountDTO.Email;
                }
                else
                {
                    throw new ArgumentException("Email không hợp lệ. Email phải có đuôi '@gmail.com'.");
                }
            }

            if (!string.IsNullOrWhiteSpace(accountDTO.ImageUrl))
            {
                userEntity.ImageUrl = accountDTO.ImageUrl;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.IdentificationCard))
            {
                userEntity.IdentificationCard = accountDTO.IdentificationCard;
            }
            //if (accountDTO.IdentificationCard.HasValue)
            //{
            //    userEntity.IdentificationCard = accountDTO.IdentificationCard.Value;
            //}
            if (accountDTO.WalletId.HasValue)
            {
                userEntity.WalletId = accountDTO.WalletId.Value;
            }
            if (accountDTO.IsUpdated.HasValue)
            {
                userEntity.IsUpdated = true;
            }

            if (accountDTO.IsActive.HasValue)
            {
                userEntity.IsActive = accountDTO.IsActive.Value;
            }
            if (!string.IsNullOrWhiteSpace(accountDTO.RankLevel))
            {
                userEntity.RankLevel = accountDTO.RankLevel;
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
                userEntity.ImageUrl = newImageUrl;
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
        public async Task ChangeChangeStatust(ChangeStatus account , int userId)
        {
            // Fetch the user entity by ID
            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(userId));
            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (account.Status.HasValue)
            {
                userEntity.Status = account.Status.Value;
            }
            else
            {
                throw new ArgumentException("Status cannot be null.");
            }
            userEntity.ModificationDate = DateTime.Now;
            _unitOfWork.UserRepository.Update(userEntity);
            await _unitOfWork.SaveAsync();
        }


        public User GetUserByID(int id)
        {
            return _unitOfWork.UserRepository.GetByID(id);
        }
        public async Task<int> CountAccountByStatus(int status)
        {
            var userCount = await Task.FromResult(_unitOfWork.UserRepository.Get(
                filter: o =>  o.Status == status
            ).Count());

            return userCount;
        }
        public async Task UpdateRankVler(UpdateRankVler account)
        {
            var userEntity = await Task.FromResult(_unitOfWork.UserRepository.GetByID(account.UserId));
            if (userEntity == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            if (string.IsNullOrWhiteSpace(account.RankLevel))
            {
                throw new ArgumentException("Password not null.");
            }
            userEntity.RankLevel = "UserEPPO";
            userEntity.ModificationDate = DateTime.Now;
            _unitOfWork.UserRepository.Update(userEntity);
            await _unitOfWork.SaveAsync();
        }
    }
}
