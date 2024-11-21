using AutoMapper;
using BusinessObjects.Models;
using DTOs.HistoryBid;
using Microsoft.EntityFrameworkCore.Migrations;
using Repository.Interfaces;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implements
{
    public class HistoryBidService : IHistoryBidService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HistoryBidService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public void CreateHistoryBid(HistoryBid bid)
        {
             _unitOfWork.HistoryBidRepository.Insert(bid);
             _unitOfWork.Save();
        }

        public IEnumerable<HistoryBidVM> GetHistoryBidsByRoomId(int pageIndex, int pageSize, int roomId)
        {
            var historyBids = _unitOfWork.HistoryBidRepository.Get(
                filter: bid => bid.RoomId == roomId,
                pageIndex: pageIndex,
                pageSize: pageSize,
                includeProperties: "User"
            );

            return _mapper.Map<IEnumerable<HistoryBidVM>>(historyBids);
        }
    }
}
