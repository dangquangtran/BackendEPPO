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
    public class RoomParticipantService: IRoomParticipantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoomParticipantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<RoomParticipant>> GetListRoomParticipant(int page, int size)
        {
            return await _unitOfWork.RoomParticipantRepository.GetAsync(includeProperties: "User,Room", pageIndex: page, pageSize: size);
        }

        public async Task<RoomParticipant> GetRoomParticipantByID(int Id)
        {
            return await Task.FromResult(_unitOfWork.RoomParticipantRepository.GetByID(Id));
        }

     
    }
}
