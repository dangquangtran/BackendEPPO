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
            return await _unitOfWork.RoomRepository.GetAsync(filter: c => c.Status == 2
                && c.EndDate >= DateTime.Now,
                orderBy: query => query.OrderByDescending(c => c.RoomId), pageIndex: page, pageSize: size, includeProperties: "Plant,Plant.ImagePlants,UserRooms");
        }
        public async Task<IEnumerable<Room>> GetListRooms(int page, int size, int userId)
        {
            // Lấy danh sách phòng đã mở với trạng thái "active" và thời gian kết thúc >= hiện tại
            var rooms = await _unitOfWork.RoomRepository.GetAsync(
                filter: c => c.Status == 2 && c.EndDate >= DateTime.Now,
                orderBy: query => query.OrderByDescending(c => c.RoomId),
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant,Plant.ImagePlants,UserRooms"
            );

            // Lấy danh sách UserRoom mà người dùng đã tham gia và có trạng thái là "active"
            var userRooms = await _unitOfWork.UserRoomRepository.GetAsync(
                filter: ur => ur.UserId == userId && ur.IsActive == true, // Kiểm tra phòng đã đăng ký và trạng thái hoạt động
                includeProperties: "Room"
            );


            return rooms;
        }

        public async Task<IEnumerable<Room>> GetListRoomsManager(int page, int size)
        {
            return await _unitOfWork.RoomRepository.GetAsync(filter: c => c.Status != 0,
                orderBy: query => query.OrderByDescending(c => c.RoomId), pageIndex: page, pageSize: size, includeProperties: "Plant,Plant.ImagePlants");
        }
        public async Task<IEnumerable<Room>> GetListRoomsByStatus(int page, int size, int status)
        {
            return await _unitOfWork.RoomRepository.GetAsync(filter: c => c.Status == status, orderBy: query => query.OrderByDescending(c => c.RoomId), pageIndex: page, pageSize: size, includeProperties: "Plant,Plant.ImagePlants");
        }
        public async Task<IEnumerable<Room>> GetListRoomActive(int page, int size)
        {
            DateTime currentDate = DateTime.Now;

            return await _unitOfWork.RoomRepository.GetAsync(
                    filter: c => c.Status == 2 && c.RegistrationEndDate >= currentDate,
                    orderBy: query => query.OrderByDescending(c => c.RoomId),
                    pageIndex: page,
                    pageSize: size,
                    includeProperties: "Plant,Plant.ImagePlants"
                );
        }

        public async Task<IEnumerable<Room>> GetListRoomsByDateNow(int page, int size)
        {
            DateTime currentDate = DateTime.UtcNow.AddHours(7);
            return await _unitOfWork.RoomRepository.GetAsync(
               pageIndex: page,
               pageSize: size,
               includeProperties: "Plant,Plant.ImagePlants",
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
                includeProperties: "Plant,Plant.ImagePlants"
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
                includeProperties: "Plant,Plant.ImagePlants",
                orderBy: q => isDescending
                                ? q.OrderByDescending(r => r.Plant.Price)
                                : q.OrderBy(r => r.Plant.Price)
            );
        }

        public async Task<IEnumerable<Room>> GetListRoomsIsActive(int page, int size)
        {
            return await _unitOfWork.RoomRepository.GetAsync(
                filter: c => c.Status == 2 &&
                             c.ActiveDate <= DateTime.UtcNow.AddHours(7) &&
                             c.EndDate >= DateTime.UtcNow.AddHours(7),
                pageIndex: page,
                pageSize: size,
                includeProperties: "Plant,Plant.ImagePlants"
            );
        }




        public async Task<Room> GetRoomByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RoomRepository.GetByID(Id, includeProperties: "Plant,Plant.ImagePlants"));
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
            plant.IsActive = false;

            var entity = new Room
            {
                PlantId = room.PlantId,
                RegistrationOpenDate = room.RegistrationOpenDate,
                RegistrationEndDate = room.RegistrationEndDate,
                RegistrationFee = room.RegistrationFee,
                PriceStep = room.PriceStep,
                CreationDate = DateTime.UtcNow.AddHours(7),
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
            entity.ModificationDate = DateTime.UtcNow.AddHours(7);
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

        public async Task<IEnumerable<object>> GetListHistoryRooms(int userId, int page, int size)
        {
            // Lấy danh sách phòng đã kết thúc đấu giá và có trạng thái thành công
            var rooms = await _unitOfWork.RoomRepository.GetAsync(
                filter: r => r.Status == 3 ,
                orderBy: query => query.OrderByDescending(r => r.EndDate), // Sắp xếp theo ngày kết thúc
                pageIndex: page,
                pageSize: size,
                includeProperties: "UserRooms,HistoryBids,Plant,Plant.ImagePlants" // Bao gồm các thông tin cần thiết
            );

            // Duyệt qua danh sách phòng để kiểm tra người dùng có tham gia phòng và có thắng hay không
            var historyRooms = rooms.Select(room => new
            {
                RoomId = room.RoomId,
                RoomName = room.Plant?.PlantName, // Lấy tên cây đấu giá
                Image = room.Plant.MainImage,
                RegistrationFee = room.RegistrationFee,
                PriceStep = room.PriceStep,
                EndDate = room.EndDate,
                UserBidAmount = room.HistoryBids
                    .Where(bid => bid.UserId == userId) // Lọc giá trị mà người dùng đã nhập
                    .OrderByDescending(bid => bid.BidTime) // Sắp xếp theo thời gian nhập gần nhất
                    .FirstOrDefault()?.BidAmount, // Lấy giá trị đấu giá gần nhất của người dùng
                UserIsWinner = room.HistoryBids.Any(bid => bid.IsWinner == true && bid.UserId == userId), // Kiểm tra nếu người dùng thắng
                WinningBid = room.HistoryBids
                    .Where(bid => bid.IsWinner == true)
                    .OrderByDescending(bid => bid.BidTime)
                    .FirstOrDefault()?.BidAmount, // Lấy giá trị đấu giá thắng cuộc
                UserHasRegistered = room.UserRooms.Any(ur => ur.UserId == userId && ur.Status == 1) // Kiểm tra nếu người dùng đã đăng ký tham gia phòng này
            });

            // Lọc ra chỉ những phòng đấu giá mà người dùng đã đăng ký tham gia
            var filteredHistoryRooms = historyRooms.Where(room => room.UserHasRegistered);

            return filteredHistoryRooms;
        }


    }
}
