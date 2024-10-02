using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<Room>> GetListRooms(int page, int size);
        Task<Room> GetRoomByID(int Id);
    }
}
