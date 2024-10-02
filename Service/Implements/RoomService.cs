using BusinessObjects.Models;
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
    }
}
