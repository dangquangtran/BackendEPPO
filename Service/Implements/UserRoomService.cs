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
            return await _unitOfWork.UserRoomRepository.GetAsync(filter: c => c.Status != 0, pageIndex: page, pageSize: size);
        }
        public async Task<IEnumerable<UserRoom>> GetListUserRoomWithUserToken(int page, int size, int userId)
        {
            return await _unitOfWork.UserRoomRepository.GetAsync(
                filter: c => c.UserId == userId && c.Status != 0 && c.IsActive != false,
                pageIndex: page, pageSize: size,
                includeProperties: "Room.Plant");
        }

        public async Task<UserRoom> GetUserRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(Id, includeProperties: "Room.Plant"));
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
            var entity = new UserRoom
            {
                RoomId = userRoom.RoomId,
                UserId = userID,
                JoinDate = DateTime.Now,
                IsActive = true,
                Status = 1,
            };
            _unitOfWork.UserRoomRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteUserRoom(DeleteUserRoomDTO userRoom)
        {
            var entity = await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(userRoom.UserRoomId));

            if (entity == null)
            {
                throw new Exception($"User Room with ID {userRoom.UserRoomId} not found.");
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
            var timeSpan = room.ActiveDate.Value - DateTime.Now;

           
            return (int)timeSpan.TotalSeconds; 
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
            var timeSpan = room.EndDate.Value - room.ActiveDate.Value;


            return (int)timeSpan.TotalSeconds;
        }

    }
}
