using BusinessObjects.Models;
using DTOs.HistoryBid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IHistoryBidService
    {
        void CreateHistoryBid(HistoryBid bid);
        IEnumerable<HistoryBidVM> GetHistoryBidsByRoomId(int pageIndex, int pageSize, int roomId);
    }
}
