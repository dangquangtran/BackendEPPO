using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DTOs.Room;
namespace Service.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetListRooms(int page, int size);
        Task<IEnumerable<Room>> GetListRoomsByDateNow(int page, int size);
        Task<Room> GetRoomByID(int Id);
        Task UpdateRoom(UpdateRoomDTO room);
        Task CreateRoom(CreateRoomDTO room);
        Task DeleteRoom(DeleteRoomDTO room , int roomId);
        Task<IEnumerable<Room>> SearchListRoomByDate(int page, int size, string date);
        Task<IEnumerable<Room>> FilterListRoomByPrice(int page, int size, double? minPrice = null, double? maxPrice = null, bool isDescending = false);
        Task<int> CountTimeClose(int roomId);
        Task<int> CountTimeActive(int roomId);
        Task<int> CountUserRegister(int roomId);
        Task<IEnumerable<Room>> GetListRoomsIsActive(int page, int size);
        Task UpdateStatusRoom(UpdateStatusRoomDTO room, int roomId);
        Task<IEnumerable<Room>> GetListRoomsByStatus(int page, int size, int status);
    }
}
