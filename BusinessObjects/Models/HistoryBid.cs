using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class HistoryBid
    {
        public int HistoryBidId { get; set; }
        public int? UserId { get; set; }
        public int? RoomId { get; set; }
        public double? BidAmount { get; set; }
        public double? PriceAuctionNext { get; set; }
        public DateTime? BidTime { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Code { get; set; }
        public bool? IsWinner { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPayment { get; set; }
        public int? Status { get; set; }

        public virtual Room Room { get; set; }
        public virtual User User { get; set; }
    }
}
