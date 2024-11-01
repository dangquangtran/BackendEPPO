using BusinessObjects.Models;
using DTOs.Room;
using DTOs.Wallet;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Room>> GetListRooms(int page, int size)
        {
            return await _unitOfWork.RoomRepository.GetAsync(pageIndex: page, pageSize: size);
        }

        public async Task<Room> GetRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RoomRepository.GetByID(Id));
        }
        public async Task CreateRoom(CreateRoomDTO room)
        {
            var entity = new Room
            {
                PlantId = room.PlantId,
                CreationDate = DateTime.Now,
                ActiveDate = room.ActiveDate,
                EndDate = room.EndDate,
                ModificationDate = room.ModificationDate,
                ModificationBy = room.ModificationBy,
                Status = 1,
            };
            _unitOfWork.RoomRepository.Insert(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateRoom(UpdateRoomDTO room)
        {

            var entity = await Task.FromResult(_unitOfWork.RoomRepository.GetByID(room.RoomId));

            if (entity == null)
            {
                throw new Exception($"Room with ID {room.RoomId} not found.");
            }
            room.PlantId = room.PlantId;
            room.CreationDate = room.CreationDate;
            room.ActiveDate = room.ActiveDate;
            room.EndDate = room.EndDate;
            room.ModificationDate = room.ModificationDate;
            room.ModificationBy = room.ModificationBy;
            room.Status = room.Status;
            _unitOfWork.RoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
    }
}
