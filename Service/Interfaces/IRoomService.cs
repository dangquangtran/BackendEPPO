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
    }
}
