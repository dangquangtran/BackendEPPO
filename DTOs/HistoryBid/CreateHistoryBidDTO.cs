using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.HistoryBid
{
    public class CreateHistoryBidDTO
    {
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public double? BidAmount { get; set; }
    }
}
