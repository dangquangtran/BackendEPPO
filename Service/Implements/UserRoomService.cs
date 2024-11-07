using BusinessObjects.Models;
using DTOs.Room;
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
            return await _unitOfWork.UserRoomRepository.GetAsync(pageIndex: page, pageSize: size); 
        }

        public async Task<UserRoom> GetUserRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(Id));
        }

        public async Task UpdateUserRoom(UpdateUserRoomDTO userRoom)
        {
            var entity = await Task.FromResult(_unitOfWork.UserRoomRepository.GetByID(userRoom.UserRoomId));

            if (entity == null)
            {
                throw new Exception($"User Room with ID {userRoom.UserRoomId} not found.");
            }
            userRoom.RoomId = userRoom.RoomId;
            userRoom.UserId = userRoom.UserId;
            userRoom.JoinDate = userRoom.JoinDate;
            userRoom.IsActive = userRoom.IsActive;
            userRoom.Status = userRoom.Status;
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
    }
}
