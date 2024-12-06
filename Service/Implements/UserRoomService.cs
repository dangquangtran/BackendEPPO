using BusinessObjects.Models;
using DTOs.Room;
using PdfSharp.Pdf.Filters;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class UserRoomService : IUserRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserRoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserRoom>> GetListUserRoom(int page, int size)
        {
            return await _unitOfWork.UserRoomRepository.GetAsync(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.UserRoomId), pageIndex: page, pageSize: size);
        }
        public async Task<IEnumerable<UserRoom>> GetListUserRoomWithUserToken(int page, int size, int userId)
        {
            return await _unitOfWork.UserRoomRepository.GetAsync(
                filter: c => c.UserId == userId && c.Status !=0  && c.IsActive != false && c.Room.Status == 2,
                pageIndex: page, pageSize: size,
                orderBy: query => query.OrderByDescending(c => c.UserRoomId),
                includeProperties: "Room.Plant");
        }

        public async Task<UserRoom> GetUserRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(Id, includeProperties: "Room.Plant"));
        }

        public async Task<UserRoom> GetUserRoomByRoomID(int roomId)
        {
       
            var userRoom =  _unitOfWork.UserRoomRepository
                .Get(filter: ur => ur.RoomId == roomId, includeProperties: "Room.Plant");

            return userRoom.FirstOrDefault();
        }


        public async Task UpdateUserRoom(UpdateUserRoomDTO userRoom)
        {
            var entity = await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(userRoom.UserRoomId));

            if (entity == null)
            {
                throw new Exception($"User Room with ID {userRoom.UserRoomId} not found.");
            }
            //entity.RoomId = userRoom.RoomId;
            entity.UserId = userRoom.UserId;
            entity.JoinDate = userRoom.JoinDate;
            entity.IsActive = userRoom.IsActive;
            entity.Status = userRoom.Status;
            _unitOfWork.UserRoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task CreateUserRoom(CreateUserRoomDTO userRoom, int userID)
        {
            // Lấy thông tin phòng đấu giá từ database
            var room = await _unitOfWork.RoomRepository.GetFirstOrDefaultAsync(
                filter: r => r.RoomId == userRoom.RoomId && r.Status == 2
            );

            if (room == null)
            {
                throw new Exception("Phòng đấu giá không tồn tại hoặc đã bị hủy.");
            }    
            // Kiểm tra thời gian đăng ký
            //if (DateTime.UtcNow < room.RegistrationOpenDate || DateTime.UtcNow > room.RegistrationEndDate)
            //{
            //    throw new Exception("Thời gian đăng ký đã hết hạn.");
            //}


            // Kiểm tra xem người dùng đã đăng ký phòng này chưa
            var existingRegistration = await _unitOfWork.UserRoomRepository.GetFirstOrDefaultAsync(
                filter: ur => ur.RoomId == userRoom.RoomId && ur.UserId == userID && ur.Status == 1
            );
            if (existingRegistration != null)
            {
                throw new Exception("Người chơi đã đăng kí tham gia phòng này rồi! .");
            }


            // Kiểm tra phí đăng ký
            var userWallet = await _unitOfWork.WalletRepository.GetFirstOrDefaultAsync(
                filter: w => w.Users.Any(u => u.UserId == userID)
            );

            if (userWallet == null || userWallet.NumberBalance < room.RegistrationFee)
            {
                throw new Exception("Số dư ví không đủ để đăng ký phòng.");
            }

            // Trừ phí đăng ký từ ví người dùng
            userWallet.NumberBalance -= room.RegistrationFee;

            // Lưu giao dịch giảm số dư ví
            var transaction = new Transaction
            {
                WalletId = userWallet.WalletId,
                WithdrawNumber = room.RegistrationFee,
                WithdrawDate = DateTime.UtcNow.AddHours(7),
                Status = 1,
                CreationDate = DateTime.UtcNow.AddHours(7),
                IsActive = true,
                Description = "Đăng ký phòng đấu giá"
            };
            _unitOfWork.TransactionRepository.Insert(transaction);

            var entity = new UserRoom
            {
                RoomId = userRoom.RoomId,
                UserId = userID,
                JoinDate = DateTime.UtcNow.AddHours(7),
                IsActive = true,
                Status = 1,
            };

            _unitOfWork.UserRoomRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteUserRoom(DeleteUserRoomDTO userRoom , int userRoomId)
        {
            var entity = await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(userRoomId));

            if (entity == null)
            {
                throw new Exception($"User Room with ID {userRoomId} not found.");
            }

            entity.Status = 0;
            _unitOfWork.UserRoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task<int> CountUserRegister(int roomId)
        {
            var uCount = await Task.FromResult(_unitOfWork.UserRoomRepository.Get(
                filter: o => o.Status != 0 && o.RoomId == roomId
            ).Count());

            return uCount;
        }
        public async Task<int> CountTimeActive(int roomId)
        {
            var room = await Task.FromResult(
                _unitOfWork.RoomRepository.Get(filter: r => r.RoomId == roomId).FirstOrDefault()
            );

            if (room == null || !room.RegistrationOpenDate.HasValue || !room.RegistrationEndDate.HasValue)
            {
                throw new ArgumentNullException("Room or registration dates are invalid.");
            }
            var timeSpan = room.ActiveDate.Value - DateTime.UtcNow.AddHours(7);


            return timeSpan.TotalSeconds > 0 ? (int)timeSpan.TotalSeconds : 0;
        }
        public async Task<int> CountTimeClose(int roomId)
        {
            var room = await Task.FromResult(
                _unitOfWork.RoomRepository.Get(filter: r => r.RoomId == roomId).FirstOrDefault()
            );

            if (room == null || !room.RegistrationOpenDate.HasValue || !room.RegistrationEndDate.HasValue)
            {
                throw new ArgumentNullException("Room or registration dates are invalid.");
            }
            var timeSpan = room.EndDate.Value - DateTime.UtcNow.AddHours(7);


            return timeSpan.TotalSeconds > 0 ? (int)timeSpan.TotalSeconds : 0;
        }


    }
}
