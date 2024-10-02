using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IRoomParticipantService
    {
        Task<IEnumerable<RoomParticipant>> GetListRoomParticipant(int page, int size);
        Task<RoomParticipant> GetRoomParticipantByID(int Id);
    }
}
