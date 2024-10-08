using System;
using System.Collections.Generic;

namespace BusinessObjects.Models
{
    public partial class SubOrderDetail
    {
        public int SubOrderDetailId { get; set; }
        public int OrderDetailId { get; set; }
        public int? PlantId { get; set; }
        public int? AccessoryId { get; set; }
        public int? ServiceId { get; set; }
        public int? HistoryBidId { get; set; }

        public virtual Accessory Accessory { get; set; }
        public virtual HistoryBid HistoryBid { get; set; }
        public virtual OrderDetail OrderDetail { get; set; }
        public virtual Plant Plant { get; set; }
        public virtual Epposervice Service { get; set; }
    }
}
