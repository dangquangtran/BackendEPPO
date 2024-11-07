﻿using BusinessObjects.Models;
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
            return await _unitOfWork.RoomRepository.GetAsync(pageIndex: page, pageSize: size, includeProperties: "Plant");
        }
        public async Task<IEnumerable<Room>> GetListRoomsByDateNow(int page, int size)
        {
            DateTime currentDate = DateTime.UtcNow;
            return await _unitOfWork.RoomRepository.GetAsync(
               pageIndex: page,
               pageSize: size,
               includeProperties: "Plant",
               orderBy: q => q.OrderByDescending(r => r.ActiveDate),
               filter: r => r.ActiveDate <= currentDate 
           );
        }

        public async Task<IEnumerable<Room>> SearchListRoomByDate(int page, int size, string date)
        {
         
            if (!DateTime.TryParseExact(date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
            {
                throw new ArgumentException("Invalid date format. Please use 'yyyy-MM-dd'.");
            }

            return await _unitOfWork.RoomRepository.GetAsync(
                filter: r => r.ActiveDate.HasValue && r.ActiveDate.Value.Date == parsedDate.Date,
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant"
            );
        }
        public async Task<IEnumerable<Room>> FilterListRoomByPrice(int page, int size, double? minPrice = null, double? maxPrice = null, bool isDescending = false)
        {
            return await _unitOfWork.RoomRepository.GetAsync(
                filter: r =>
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





        public async Task<Room> GetRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RoomRepository.GetByID(Id, includeProperties: "Plant"));
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
