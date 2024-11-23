using DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs.HistoryBid
{
    public class HistoryBidVM
    {
        public int HistoryBidId { get; set; }
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public double? BidAmount { get; set; }
        public double? PriceAuctionNext { get; set; }
        public DateTime? BidTime { get; set; }
        public bool? IsWinner { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPayment { get; set; }
        public int? Status { get; set; }
        public virtual ResponseUserDTO User { get; set; }
    }
}
