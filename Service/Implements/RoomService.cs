using BusinessObjects.Models;
using DTOs.Room;
using DTOs.Wallet;
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
    public class RoomService : IRoomService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Room>> GetListRooms(int page, int size)
        {
            return await _unitOfWork.RoomRepository.GetAsync(filter: c => c.Status != 0, orderBy: query => query.OrderByDescending(c => c.RoomId), pageIndex: page, pageSize: size, includeProperties: "Plant");
        }
        public async Task<IEnumerable<Room>> GetListRoomsByDateNow(int page, int size)
        {
            DateTime currentDate = DateTime.UtcNow;
            return await _unitOfWork.RoomRepository.GetAsync(
               pageIndex: page,
               pageSize: size,
               includeProperties: "Plant,ImagePlant",
               orderBy: q => q.OrderByDescending(r => r.ActiveDate),
               filter: r => r.ActiveDate <= currentDate &&  r.Status != 0
           );
        }

        public async Task<IEnumerable<Room>> SearchListRoomByDate(int page, int size, string date)
        {
         
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid date format. Please use 'yyyy-MM-dd'.");
            }

            return await _unitOfWork.RoomRepository.GetAsync(
                filter: r => r.Status != 0 && r.ActiveDate.HasValue && r.ActiveDate.Value.Date == parsedDate.Date,
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant,ImagePlant"
            );
        }
        public async Task<IEnumerable<Room>> FilterListRoomByPrice(int page, int size, double? minPrice = null, double? maxPrice = null, bool isDescending = false)
        {
            return await _unitOfWork.RoomRepository.GetAsync(
                filter: r => r.Status != 0 &&
                             (!minPrice.HasValue || r.Plant.Price >= minPrice.Value) &&
                             (!maxPrice.HasValue || r.Plant.Price <= maxPrice.Value),
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant",
                orderBy: q => isDescending
                                ? q.OrderByDescending(r => r.Plant.Price)
                                : q.OrderBy(r => r.Plant.Price)
            );
        }

        public async Task<IEnumerable<Room>> GetListRoomsIsActive(int page, int size)
        {
            return await _unitOfWork.RoomRepository.GetAsync(
                filter: c => c.Status == 2 &&
                             c.ActiveDate <= DateTime.Now &&
                             c.EndDate >= DateTime.Now,
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant,ImagePlant"
            );
        }




        public async Task<Room> GetRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RoomRepository.GetByID(Id, includeProperties: "Plant"));
        }
        public async Task CreateRoom(CreateRoomDTO room)
        {
            var plant =  _unitOfWork.PlantRepository.GetByID(room.PlantId.Value);

            if (plant == null)
            {
                throw new Exception("Plant not found");
            }

            if (plant.TypeEcommerceId != 3)
            {
                throw new Exception("Type Ecommerce not is Auction.");
            }


            var entity = new Room
            {
                PlantId = room.PlantId,
                RegistrationOpenDate = room.RegistrationOpenDate,
                RegistrationEndDate = room.RegistrationEndDate,
                RegistrationFee = room.RegistrationFee,
                PriceStep = room.PriceStep,
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
            entity.PlantId = room.PlantId;
            entity.RegistrationOpenDate = room.RegistrationOpenDate;
            entity.RegistrationEndDate = room.RegistrationEndDate;
            entity.RegistrationFee = room.RegistrationFee;
            entity.PriceStep = room.PriceStep;
            entity.CreationDate = room.CreationDate;
            entity.ActiveDate = room.ActiveDate;
            entity.EndDate = room.EndDate;
            entity.ModificationDate = DateTime.Now;
            entity.ModificationBy = room.ModificationBy;
            entity.Status = room.Status;

            _unitOfWork.RoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteRoom(DeleteRoomDTO room, int roomId)
        {

            var entity = await Task.FromResult(_unitOfWork.RoomRepository.GetByID(roomId));

            if (entity == null)
            {
                throw new Exception($"Room with ID {room.RoomId} not found.");
            }
            entity.Status = 0;
            _unitOfWork.RoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
        }
        public async Task UpdateStatusRoom(UpdateStatusRoomDTO room , int roomId)
        {

            var entity = await Task.FromResult(_unitOfWork.RoomRepository.GetByID(roomId));

            if (entity == null)
            {
                throw new Exception($"Room with ID {roomId} not found.");
            }
            entity.Status = room.Status;
            _unitOfWork.RoomRepository.Update(entity);
            await _unitOfWork.SaveAsync();
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
            var timeSpan = room.EndDate.Value - room.ActiveDate.Value;


            return timeSpan.TotalSeconds > 0 ? (int)timeSpan.TotalSeconds : 0;
        }

        public async Task<int> CountUserRegister(int roomId)
        {
            var uCount = await Task.FromResult(_unitOfWork.UserRoomRepository.Get(
                filter: o => o.Status != 0 && o.RoomId == roomId
            ).Count());

            return uCount;
        }
    }
}
